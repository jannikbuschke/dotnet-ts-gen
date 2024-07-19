module Test.Generics

open Expecto
open Xunit

type GenericRecord<'t> = { GenericProperty: 't }

type Localizable<'a> =
  { Value: 'a
    Localizations: Map<string, 'a> }

[<Fact>]
let ```not render typedef Localizable<string>`` () =
  // runtime type, should not be rendered (only generic typedefinition)
  let rendered, _ = renderTypeAndValue typeof<Localizable<string>>

  Expect.similar rendered ""

let rendered, value = renderTypeAndValue typedefof<Localizable<string>>

[<Fact>]
let ```render typedefof Localizable<string>`` () =
  Expect.similar
    rendered
    """
export type Localizable<a> = {
  value: a
  localizations: Microsoft_FSharp_Collections.FSharpMap<System.String,a>
}
"""

// [<Fact>]
// let ```render value Localizable<string>`` () =
//   Expect.similar
//     value
//     """
// export var defaultLocalizable: <a>(defaulta:a) => Localizable<a> = <a>(defaulta:a) => ({
//  value: defaulta,
//  localizations: ({})
// })
// """

type Localizable2<'a> =
  { Value: 'a
    Localizations: Map<string, 'a> }

// recursive generic

type TreeNodeBase<'t>={
  NodeId:string
  Name:string
  Value:'t
  Children: TreeNodeBase<'t> list
  ParentId:string option
  PruneCondition:bool option
}


[<Fact>]
let ```render resursive generic`` () =
  let rendered = renderTypeDef typedefof<TreeNodeBase<string>>
  Expect.similar
    rendered
    """
export type TreeNodeBase<t> = {
 nodeId: System.String
 name: System.String
 value: t
 children: Microsoft_FSharp_Collections.FSharpList<TreeNodeBase<t>>
 parentId: Microsoft_FSharp_Core.FSharpOption<System.String>
 pruneCondition: Microsoft_FSharp_Core.FSharpOption<System.Boolean>
}
"""


//
// 	public class TreeNodeBase<T>
// 	{
// 		public TreeNodeBase() { }
//
// 		// NodeId muss™ im Context des Trees unique sein
// 		public string NodeId { get; set; }
//
// 		public string Name { get; set; }
//
// 		public T Value { get; set; }
//
// 		public List<TreeNodeBase<T>>? Children { get; set; }
//
// 		public string? ParentId { get; set; }
//
// 		public bool? PruneCondition { get; set; }
// 	}
