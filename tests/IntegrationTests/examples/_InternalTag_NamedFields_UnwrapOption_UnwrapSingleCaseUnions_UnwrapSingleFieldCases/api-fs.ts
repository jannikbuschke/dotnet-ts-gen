/* eslint-disable */

import * as ___ from './___';
import * as Microsoft_AspNetCore_Mvc from './Microsoft_AspNetCore_Mvc';
import * as System from './System';

export type Mutation = {
  '/api/user/create': {
    POST: [
      ___.f__AnonymousType2325210262<System.String>,
      Microsoft_AspNetCore_Mvc.ProblemDetails,
    ];
  };
};
export type Query = {
  '/api/user/get': {
    GET: [
      ___.f__AnonymousType2325210262<System.String>,
      Microsoft_AspNetCore_Mvc.ProblemDetails,
    ];
  };
  '/user/get-name': {
    GET: [System.String, System.String];
  };
};
