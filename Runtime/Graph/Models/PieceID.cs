using System;
using Ceres.Graph;

namespace NextGenDialogue.Graph
{
    [Serializable]
    public class PieceID : SharedVariable<string>
    {
        public PieceID()
        {
            IsShared = true;
            IsExposed = false;
        }
        
        protected override SharedVariable<string> CloneT()
        {
            return new PieceID { Value = value };
        }
    }
}
