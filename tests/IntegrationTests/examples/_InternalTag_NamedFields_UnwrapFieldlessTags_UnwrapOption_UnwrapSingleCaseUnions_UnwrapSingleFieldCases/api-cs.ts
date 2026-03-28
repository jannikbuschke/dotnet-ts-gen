/* eslint-disable */

import * as CSharpWebapp_Controllers from './CSharpWebapp_Controllers';
import * as CSharpWebapp_Controllers2 from './CSharpWebapp_Controllers2';
import * as CSharpWebapp_Controllers2_StaticClass from './CSharpWebapp_Controllers2_StaticClass';
import * as CSharpWebapp_Controllers__CSharpWebapp_Controllers from './CSharpWebapp_Controllers__CSharpWebapp_Controllers';
import * as Microsoft_AspNetCore_Mvc from './Microsoft_AspNetCore_Mvc';
import * as Microsoft_FSharp_Core from './Microsoft_FSharp_Core';
import * as NamespaceA from './NamespaceA';
import * as System from './System';
import * as System_Dynamic from './System_Dynamic';
import * as WebApplication5_ModuleClass from './WebApplication5_ModuleClass';

export type Query = {
  '/fsharp/get-result': {
    GET: [
      Microsoft_FSharp_Core.Unit,
      Microsoft_FSharp_Core.FSharpResult<System.String, System.String>,
    ];
  };
  '/fsharp/get-result-async': {
    GET: [
      Microsoft_FSharp_Core.Unit,
      Microsoft_FSharp_Core.FSharpResult<System.String, System.String>,
    ];
  };
  '/api2/get2': {
    GET: [
      Microsoft_FSharp_Core.Unit,
      CSharpWebapp_Controllers2_StaticClass.ResponseDto2,
    ];
  };
  '/api2/get': {
    GET: [Microsoft_FSharp_Core.Unit, CSharpWebapp_Controllers2.ResponsDto];
  };
  '/api2/get3': {
    GET: [Microsoft_FSharp_Core.Unit, WebApplication5_ModuleClass.Dto];
  };
  '/api/index': {
    GET: [Microsoft_FSharp_Core.Unit, System.String];
  };
  '/api/get-dynamic': {
    GET: [Microsoft_FSharp_Core.Unit, System_Dynamic.ExpandoObject];
  };
  '/api/get': {
    GET: [Microsoft_FSharp_Core.Unit, Microsoft_AspNetCore_Mvc.ActionResult];
  };
  '/api/get-class-with-many-deps': {
    GET: [Microsoft_FSharp_Core.Unit, NamespaceA.ClassWithManyDeps];
  };
  '/api/multiple-query-parameters2': {
    GET: [
      CSharpWebapp_Controllers__CSharpWebapp_Controllers._MultipleQueryParameters2_Request,
      Microsoft_AspNetCore_Mvc.ActionResult,
    ];
  };
  '/api/multiple-query-parameters': {
    GET: [
      CSharpWebapp_Controllers__CSharpWebapp_Controllers._MultipleQueryParameters_Request,
      Microsoft_AspNetCore_Mvc.ActionResult,
    ];
  };
  '/api/complex-request-object': {
    GET: [
      CSharpWebapp_Controllers.RequestDto,
      Microsoft_AspNetCore_Mvc.ActionResult,
    ];
  };
};
export type Mutation = {
  '/api/index': {
    POST: [Microsoft_FSharp_Core.Unit, Microsoft_AspNetCore_Mvc.ActionResult];
  };
  '/api/post': {
    POST: [Microsoft_FSharp_Core.Unit, Microsoft_AspNetCore_Mvc.ActionResult];
  };
};
