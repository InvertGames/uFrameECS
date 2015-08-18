using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS
{
    public class HandlerNodeVisitor : IHandlerNodeVisitor
    {
        private List<ActionNode> outputtedNodes = new List<ActionNode>();

        public void Visit(IDiagramNodeItem item)
        {
            var handlerNode = item as ISequenceNode;
            var actionNode = item as ActionNode;
            var actionBranch = item as ActionBranch;
            var actionOut = item as IActionOut;
            var actionIn = item as IActionIn;
            var setVariableNode = item as SetVariableNode;
            var handlerIn = item as HandlerIn;
            if (handlerIn != null)
            {
                BeforeVisitHandlerIn(handlerIn);
                VisitHandlerIn(handlerIn);
                AfterVisitHandlerIn(handlerIn);
            }
            if (setVariableNode != null)
            {
                BeforeSetVariableHandler(setVariableNode);
                VisitSetVariable(setVariableNode);
                AfterVisitSetVariable(setVariableNode);
            }
            if (handlerNode != null)
            {
                BeforeVisitHandler(handlerNode);
                VisitHandler(handlerNode);
                AfterVisitHandler(handlerNode);
            }

            if (actionNode != null)
            {
                BeforeVisitAction(actionNode);
                VisitAction(actionNode);
                AfterVisitAction(actionNode);
            }
                
            if (actionBranch != null)
            {
                BeforeVisitBranch(actionBranch);
                VisitBranch(actionBranch);
                AfterVisitBranch(actionBranch);
            }
                
            if (actionOut != null)
            {
                BeforeVisitOutput(actionOut);
                VisitOutput(actionOut);
                AfterVisitOutput(actionOut);
            }
                
            if (actionIn != null)
            {
                BeforeVisitInput(actionIn);
                VisitInput(actionIn);
                AfterVisitInput(actionIn);
            }
                
        }

        public virtual void AfterVisitHandlerIn(HandlerIn handlerIn)
        {
            
        }

        public virtual void VisitHandlerIn(HandlerIn handlerIn)
        {
                
        }

        public virtual void BeforeVisitHandlerIn(HandlerIn handlerIn)
        {
                
        }

        public virtual void BeforeSetVariableHandler(SetVariableNode setVariableNode)
        {
            Visit(setVariableNode.VariableInputSlot);
            Visit(setVariableNode.ValueInputSlot);
        }

        public virtual void VisitSetVariable(SetVariableNode setVariableNode)
        {
   
        }

        private void AfterVisitSetVariable(SetVariableNode setVariableNode)
        {

        }

        public virtual void AfterVisitInput(IActionIn actionIn)
        {
            
        }

        public virtual void BeforeVisitHandler(ISequenceNode handlerNode)
        {
            var handler = handlerNode as HandlerNode;
            if (handler != null)
                foreach (var item in handler.HandlerInputs)
                {
                    Visit(item);
                }
        }

        public virtual void AfterVisitHandler(ISequenceNode handlerNode)
        {
            
        }


        public virtual void BeforeVisitBranch(ActionBranch actionBranch)
        {
                    
        }

        public virtual void AfterVisitBranch(ActionBranch actionBranch)
        {
                
        }

        public virtual void BeforeVisitOutput(IActionOut actionOut)
        {
                
        }

        public virtual void AfterVisitOutput(IActionOut actionIn)
        {
                
        }

        public virtual void BeforeVisitInput(IActionIn actionIn)
        {
            var actionOutput = actionIn.InputFrom<ActionOut>();
            if (actionOutput == null) return;
            var actionNode = actionOutput.Node as ActionNode;

            if (actionNode != null)
            {
                if (outputtedNodes.Contains(actionNode)) return;

                Visit(actionNode);
                outputtedNodes.Add(actionNode);
            }
        }

        public virtual void BeforeVisitAction(ActionNode actionNode)
        {
            var outputtedNodes = new List<ActionNode>();

            

            foreach (var input in actionNode.InputVars)
            {
                Visit(input);
            }
       
        }

        public virtual void AfterVisitAction(ActionNode actionNode)
        {

            
            var hasInferredOutput = false;
            foreach (var output in actionNode.OutputVars.OfType<ActionOut>())
            {
                Visit(output);
            }
            foreach (var output in actionNode.OutputVars.OfType<ActionBranch>())
            {
                Visit(output);
                if (output.OutputTo<ActionNode>() != null)
                {
                    hasInferredOutput = true;
                }

            }
            if (!hasInferredOutput & actionNode.Right != null)
            {
                Visit(actionNode.Right);
            }
        }
        public virtual void VisitAction(ActionNode actionNode)
        {

        }

        public virtual void VisitBranch(ActionBranch output)
        {
            var item = output.OutputTo<SequenceItemNode>();
            if (item != null)
            {
                Visit(item);
            }
        }

        public virtual void VisitOutput(IActionOut output)
        {
                
        }

        public virtual void VisitInput(IActionIn input)
        {
            
        }

        public virtual void VisitHandler(ISequenceNode handlerNode)
        {
            Visit(handlerNode.Right);
        }
    }
}