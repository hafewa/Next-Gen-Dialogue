namespace NextGenDialogue
{
    public readonly struct NextPieceModule : IDialogueModule
    {
        public string NextID { get; }
        public NextPieceModule(string nextID)
        {
            NextID = nextID;
        }
    }
}
