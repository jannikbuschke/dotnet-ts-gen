/* eslint-disable */

import * as System from './System';
import * as Microsoft_FSharp_Core from './Microsoft_FSharp_Core';
import * as CSharpWebapp_Controllers2_StaticClass from './CSharpWebapp_Controllers2_StaticClass';
import * as System_Collections_Generic from './System_Collections_Generic';
import * as CSharpWebapp_Controllers2 from './CSharpWebapp_Controllers2';
import * as WebApplication5_ModuleClass from './WebApplication5_ModuleClass';
import * as Microsoft_AspNetCore_Mvc from './Microsoft_AspNetCore_Mvc';
import * as System_Dynamic from './System_Dynamic';
import * as NamespaceA from './NamespaceA';
import * as NamespaceC from './NamespaceC';
import * as NamespaceB from './NamespaceB';
import * as NamespaceD from './NamespaceD';
import * as CSharpWebapp_Controllers__CSharpWebapp_Controllers from './CSharpWebapp_Controllers__CSharpWebapp_Controllers';
import * as CSharpWebapp_Controllers from './CSharpWebapp_Controllers';
import * as FsApi from './FsApi';
import * as IntegrationTests from './IntegrationTests';
import * as ___ from './___';

export type Api = {
  '/api/controller/get-string': {
    GET: [Microsoft_FSharp_Core.Unit, System.String];
  };
  '/api/controller/get-record': {
    GET: [Microsoft_FSharp_Core.Unit, FsApi.Record];
  };
  '/api/controller/get-union': {
    GET: [Microsoft_FSharp_Core.Unit, FsApi.Union];
  };
  '/api/controller/union-task': {
    GET: [Microsoft_FSharp_Core.Unit, FsApi.Union];
  };
  '/api/controller/get-version': {
    GET: [Microsoft_FSharp_Core.Unit, System.Version];
  };
  '/api/controller/get-uri': {
    GET: [Microsoft_FSharp_Core.Unit, System.Uri];
  };
  '/api/controller/get-post-query': {
    POST: [FsApi.Record, System.String];
  };
};
