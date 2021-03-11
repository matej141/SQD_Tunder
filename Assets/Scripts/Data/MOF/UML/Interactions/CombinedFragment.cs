///////////////////////////////////////////////////////////
//  CombinedFragment.cs
//  Implementation of the Class CombinedFragment
//  Generated by Enterprise Architect
//  Created on:      04-Oct-2018 16:51:27
//  Original author: Iva
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



using UML.Interactions;
namespace UML.Interactions {
    /// <summary>
    /// A CombinedFragment defines an expression of InteractionFragments. A
    /// CombinedFragment is defined by an interaction operator and corresponding
    /// InteractionOperands. Through the use of CombinedFragments the user will be able
    /// to describe a number of traces in a compact and concise manner.
    /// </summary>
    public class CombinedFragment : InteractionFragment {

        /// <summary>
        /// Specifies the operation which defines the semantics of this combination of
        /// InteractionFragments.
        /// </summary>
        public InteractionOperatorKind interactionOperator;
        /// <summary>
        /// The set of operands of the combined fragment.
        /// </summary>
        public readonly List<UML.Interactions.InteractionOperand> operand = new List<UML.Interactions.InteractionOperand>();
        /// <summary>
        /// Specifies the gates that form the interface between this CombinedFragment and
        /// its surroundings
        /// </summary>
        public readonly List<UML.Interactions.Gate> cfragmentGate = new List<UML.Interactions.Gate>();

		public CombinedFragment(){

		}

		~CombinedFragment(){

		}

        public override string XmiType() { return "uml:CombinedFragment"; }

    }//end CombinedFragment

}//end namespace Interactions