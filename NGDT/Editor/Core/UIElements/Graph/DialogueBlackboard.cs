using System.Linq;
using Ceres;
using Ceres.Editor.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace Kurisu.NGDT.Editor
{
    public class DialogueBlackboard: CeresBlackboard
    {
        private readonly BlackboardSection _pieceIdSection;
        
        public DialogueBlackboard(CeresGraphView graphView) : base(graphView)
        {
            ScrollView.Add(_pieceIdSection = new BlackboardSection { title = "Piece IDs" });
        }

        protected override bool CanVariableExposed(SharedVariable variable)
        {
            return variable is not PieceID;
        }

        protected override BlackboardRow CreateVariableBlackboardRow(SharedVariable variable, BlackboardField blackboardField, VisualElement valueField)
        {
            var propertyView = base.CreateVariableBlackboardRow(variable, blackboardField, valueField);
            if (variable is not PieceID) return propertyView;
            
            blackboardField.RegisterCallback<ClickEvent>((evt) => FindRelatedPiece(variable));
            propertyView.Q<Button>("expandButton").RemoveFromHierarchy();
            return propertyView;
        }

        protected override void AddVariableRow(SharedVariable variable, BlackboardRow blackboardRow)
        {
            if (variable is PieceID)
            {
                _pieceIdSection.Add(blackboardRow);
                return;
            }
            base.AddVariableRow(variable, blackboardRow);
        }

        private void FindRelatedPiece(SharedVariable variable)
        {
            var piece = graphView.nodes.OfType<PieceContainer>().FirstOrDefault(x => x.GetPieceID() == variable.Name);
            if (piece != null)
            {
                graphView.AddToSelection(piece);
            }
        }
    }
}