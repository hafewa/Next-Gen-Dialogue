using System;
using System.Collections.Generic;
using System.Threading;
using Chris.Gameplay;
using Cysharp.Threading.Tasks;
namespace Kurisu.NGDS
{
    /// <summary>
    /// Dialogue <see cref="WorldSubsystem"/> implementation
    /// </summary>
    public class DialogueSystem : WorldSubsystem
    {
        public class DialogueResolverContainer
        {
            private readonly IDialogueResolver _dialogueResolver;
            
            private readonly IPieceResolver _pieceResolver;
            
            private readonly IOptionResolver _optionResolver;
            
            private ResolverModule _resolverModule;
            
            public IDialogueResolver DialogueResolver => _resolverModule.DialogueResolver ?? _dialogueResolver;
            
            public IPieceResolver PieceResolver => _resolverModule.PieceResolver ?? _pieceResolver;
            
            public IOptionResolver OptionResolver => _resolverModule.OptionResolver ?? _optionResolver;
            
            public DialogueResolverContainer()
            {
                // Collect global resolver
                _dialogueResolver = ContainerSubsystem.Get().Resolve<IDialogueResolver>() ?? new DefaultDialogueResolver();
                _pieceResolver = ContainerSubsystem.Get().Resolve<IPieceResolver>() ?? new DefaultPieceResolver();
                _optionResolver = ContainerSubsystem.Get().Resolve<IOptionResolver>() ?? new DefaultOptionResolver();
            }
            
            /// <summary>
            /// Collect dialogue specific resolver
            /// </summary>
            /// <param name="dialogue"></param>
            public void Install(Dialogue dialogue)
            {
                dialogue.TryGetModule(out _resolverModule);
            }
        }
        
        private IDialogueLookup _dialogueLookup;
        
        public bool IsPlaying => _dialogueLookup != null;
        
        public event Action<IDialogueResolver> OnDialogueStart;
        
        public event Action<IPieceResolver> OnPiecePlay;
        
        public event Action<IOptionResolver> OnOptionCreate;
        
        public event Action OnDialogueOver;
        
        private DialogueResolverContainer _resolverContainer;
        
        protected DialogueResolverContainer ResolverContainer => _resolverContainer ??= new DialogueResolverContainer();
        
        private CancellationTokenSource _cts = new();

        public static DialogueSystem Get()
        {
            return GetOrCreate<DialogueSystem>();
        }
        
        protected override void Release()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        
        public IDialogueLookup GetCurrentLookup()
        {
            return _dialogueLookup;
        }
        
        public Dialogue GetCurrentDialogue()
        {
            return _dialogueLookup?.ToDialogue();
        }
        
        public void StartDialogue(IDialogueLookup dialogueProvider)
        {
            _dialogueLookup = dialogueProvider;
            var dialogueData = dialogueProvider.ToDialogue();
            ResolverContainer.Install(dialogueData);
            ResolverContainer.DialogueResolver.Inject(dialogueData, this);
            DialogueEnterAsync().AttachExternalCancellation(_cts.Token).Forget();
        }
        
        private async UniTask DialogueEnterAsync()
        {
            await ResolverContainer.DialogueResolver.EnterDialogue().AttachExternalCancellation(_cts.Token);
            OnDialogueStart?.Invoke(ResolverContainer.DialogueResolver);
            PlayDialoguePiece(_dialogueLookup.GetFirst());
        }
        
        private void PlayDialoguePiece(Piece piece)
        {
            ResolverContainer.PieceResolver.Inject(piece, this);
            PieceEnterAsync().AttachExternalCancellation(_cts.Token).Forget();
        }
        
        private async UniTask PieceEnterAsync()
        {
            await ResolverContainer.PieceResolver.EnterPiece().AttachExternalCancellation(_cts.Token);
            OnPiecePlay?.Invoke(ResolverContainer.PieceResolver);
        }
        
        public void PlayDialoguePiece(string targetID)
        {
            PlayDialoguePiece(_dialogueLookup.GetNext(targetID));
        }
        
        public void CreateOption(IReadOnlyList<Option> options)
        {
            ResolverContainer.OptionResolver.Inject(options, this);
            OptionEnterAsync().AttachExternalCancellation(_cts.Token).Forget();
        }

        private async UniTask OptionEnterAsync()
        {
            await ResolverContainer.OptionResolver.EnterOption().AttachExternalCancellation(_cts.Token);
            OnOptionCreate?.Invoke(ResolverContainer.OptionResolver);
        }

        public void EndDialogue(bool forceEnd)
        {
            if (forceEnd)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }
            ResolverContainer.DialogueResolver.ExitDialogue();
            OnDialogueOver?.Invoke();
            _dialogueLookup = null;
        }
    }
}
