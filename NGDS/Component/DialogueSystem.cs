using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.NGDS
{
    public class ResolverHandler
    {
        private readonly IDialogueResolver dialogueResolver;
        private readonly IPieceResolver pieceResolver;
        private readonly IOptionResolver optionResolver;
        private ResolverModule resolverModule;
        public IDialogueResolver DialogueResolver => resolverModule.DialogueResolver ?? dialogueResolver;
        public IPieceResolver PieceResolver => resolverModule.PieceResolver ?? pieceResolver;
        public IOptionResolver OptionResolver => resolverModule.OptionResolver ?? optionResolver;
        public ResolverHandler()
        {
            //Collect global resolver
            dialogueResolver = IOCContainer.Resolve<IDialogueResolver>() ?? new BuiltInDialogueResolver();
            pieceResolver = IOCContainer.Resolve<IPieceResolver>() ?? new BuiltInPieceResolver();
            optionResolver = IOCContainer.Resolve<IOptionResolver>() ?? new BuiltInOptionResolver();
        }
        public void Handle(Dialogue dialogue)
        {
            //Collect dialogue specific resolver
            dialogue.TryGetModule(out resolverModule);
        }
    }
    public class DialogueSystem : MonoBehaviour, IDialogueSystem
    {

        private IProvideDialogue dialogue;
        private void Awake()
        {
            IOCContainer.Register<IDialogueSystem>(this);
        }
        private void OnDestroy()
        {
            IOCContainer.UnRegister<IDialogueSystem>(this);
        }
        public event Action<IDialogueResolver> OnDialogueStart;
        public event Action<IPieceResolver> OnPiecePlay;
        public event Action<IOptionResolver> OnOptionCreate;
        public event Action OnDialogueOver;
        private ResolverHandler resolverHandler;
        public ResolverHandler ResolverHandler
        {
            get
            {
                //Lazy initialization
                resolverHandler ??= new();
                return resolverHandler;
            }
        }
        public async void StartDialogue(IProvideDialogue dialogueProvider)
        {
            dialogue = dialogueProvider;
            var dialogueData = dialogueProvider.GetDialogue();
            ResolverHandler.Handle(dialogueData);
            ResolverHandler.DialogueResolver.Inject(dialogueData, this);
            await ResolverHandler.DialogueResolver.OnDialogueEnter();
            OnDialogueStart?.Invoke(ResolverHandler.DialogueResolver);
            PlayDialoguePiece(dialogue.GetFirst());
        }
        private async void PlayDialoguePiece(DialoguePiece piece)
        {
            ResolverHandler.PieceResolver.Inject(piece, this);
            await ResolverHandler.PieceResolver.OnPieceEnter();
            OnPiecePlay?.Invoke(ResolverHandler.PieceResolver);
        }
        public void PlayDialoguePiece(string targetID)
        {
            PlayDialoguePiece(dialogue.GetNext(targetID));
        }
        public async void CreateOption(IReadOnlyList<DialogueOption> options)
        {
            ResolverHandler.OptionResolver.Inject(options, this);
            await ResolverHandler.OptionResolver.OnOptionEnter();
            OnOptionCreate?.Invoke(ResolverHandler.OptionResolver);
        }
        public void EndDialogue()
        {
            ResolverHandler.DialogueResolver.OnDialogueExit();
            OnDialogueOver?.Invoke();
        }
    }
}