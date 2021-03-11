///////////////////////////////////////////////////////////
//  Element.cs
//  Implementation of the Class Element
//  Generated by Enterprise Architect
//  Created on:      04-Oct-2018 16:51:29
//  Original author: Iva
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



using UML.CommonStructure;
using Data.MOF;

namespace UML.CommonStructure {
	/// <summary>
	/// An Element is a constituent of a model. As such, it has the capability of
	/// owning other Elements.
	/// </summary>
	public abstract class Element : MofElement {

        /// <summary>
        /// Temporary name field. In the future it will be moved to NamedElement sub class.
        /// </summary>
        public string name = "";    // TODO Remove after NamedElement is implemented
        /// <summary>
        /// The Element that owns this Element.
        /// </summary>
        public Element owner;
		/// <summary>
		/// The Elements owned by this Element.
		/// </summary>
		public List<UML.CommonStructure.Element> ownedElement = new List<Element>();
		/// <summary>
		/// The Comments owned by this Element.
		/// </summary>
		public List<UML.CommonStructure.Comment> ownedComment = new List<Comment>();

		public Element(){

		}

		~Element(){

		}

		/// <summary>
		/// The query allOwnedElements() gives all of the direct and indirect ownedElements
		/// of an Element.
		/// </summary>
		/// <param name="result"></param>
		public LinkedList<Element> allOwnedElements(){
			return null;
		}

		/// <summary>
		/// The query mustBeOwned() indicates whether Elements of this type must have an
		/// owner. Subclasses of Element that do not require an owner must override this
		/// operation.
		/// </summary>
		/// <param name="result"></param>
		public bool mustBeOwned(){
            return true;
		}

	}//end Element

}//end namespace CommonStructure