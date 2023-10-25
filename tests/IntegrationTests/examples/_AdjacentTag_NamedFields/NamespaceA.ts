//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as System from './System';
import * as NamespaceC from './NamespaceC';
import * as NamespaceB from './NamespaceB';
import * as System_Collections_Generic from './System_Collections_Generic';
import * as NamespaceD from './NamespaceD';

export type ClassWithManyDeps = {
  myProperty: System.String;
  myBool: System.Boolean;
  myC: NamespaceC.ClassC;
  myBe: NamespaceB.ClassB;
  myBs: System_Collections_Generic.IEnumerable<NamespaceB.ClassB>;
  myBDict: System_Collections_Generic.Dictionary<
    System.String,
    NamespaceB.ClassB
  >;
  myBDict2: System_Collections_Generic.Dictionary<
    System.Int32,
    NamespaceB.ClassB
  >;
  myBDict3: System_Collections_Generic.Dictionary<
    NamespaceC.ClassC,
    NamespaceB.ClassB
  >;
  myBDict4: System_Collections_Generic.Dictionary<
    NamespaceC.ClassC,
    Array<NamespaceB.ClassB>
  >;
  myDSet: System_Collections_Generic.HashSet<NamespaceD.ClassD<System.Int32>>;
};
