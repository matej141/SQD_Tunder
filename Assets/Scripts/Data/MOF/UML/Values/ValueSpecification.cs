﻿///////////////////////////////////////////////////////////
//  ValueSpecification.cs
//  Implementation of the Class ValueSpecification
//  Generated by Enterprise Architect
//  Created on:      20-Mar-2019 11:06:27
//  Original author: Iva
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



using UML.CommonStructure;
namespace UML.Values
{
    /// <summary>
    /// A ValueSpecification is the specification of a (possibly empty) set of values.
    /// A ValueSpecification is a ParameterableElement that may be exposed as a formal
    /// TemplateParameter and provided as the actual parameter in the binding of a
    /// template.
    /// </summary>
    public abstract class ValueSpecification : Element /*PackageableElement, TypedElement*/ // TODO
    {

        /// <summary>
        /// The query booleanValue() gives a single Boolean value when one can be computed.
        /// </summary>
        /// <param name="result"></param>
        public abstract bool booleanValue();

        /// <summary>
        /// The query integerValue() gives a single Integer value when one can be computed.
        /// </summary>
        /// <param name="result"></param>
        public abstract int integerValue();

        /// <summary>
        /// The query isCompatibleWith() determines if this ValueSpecification is
        /// compatible with the specified ParameterableElement. This ValueSpecification is
        /// compatible with ParameterableElement p if the kind of this ValueSpecification
        /// is the same as or a subtype of the kind of p. Further, if p is a TypedElement,
        /// then the type of this ValueSpecification must be conformant with the type of p.
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="p"></param>
        public bool isCompatibleWith(/*ParameterableElement p*/) // TODO
        {
            return false;
            // TODO
        }

        /// <summary>
        /// The query isComputable() determines whether a value specification can be
        /// computed in a model. This operation cannot be fully defined in OCL. A
        /// conforming implementation is expected to deliver true for this operation for
        /// all ValueSpecifications that it can compute, and to compute all of those for
        /// which the operation is true. A conforming implementation is expected to be able
        /// to compute at least the value of all LiteralSpecifications.
        /// </summary>
        /// <param name="result"></param>
        public bool isComputable()
        {
            return false;
        }

        /// <summary>
        /// The query isNull() returns true when it can be computed that the value is null.
        /// </summary>
        /// <param name="result"></param>
        public bool isNull()
        {
            return false;
        }

        /// <summary>
        /// The query realValue() gives a single Real value when one can be computed.
        /// </summary>
        /// <param name="result"></param>
        public abstract double realValue();

        /// <summary>
        /// The query stringValue() gives a single String value when one can be computed.
        /// </summary>
        /// <param name="result"></param>
        public abstract string stringValue();

        /// <summary>
        /// The query unlimitedValue() gives a single UnlimitedNatural value when one can
        /// be computed.
        /// </summary>
        /// <param name="result"></param>
        //public UnlimitedValue unlimitedValue()
        //{
        //
        //}

        public ValueSpecification()
        {

        }

        ~ValueSpecification()
        {

        }

    }//end ValueSpecification

}//end namespace Values