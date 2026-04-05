/* eslint-disable */

import * as ___ from './___';
import * as Microsoft_AspNetCore_Mvc from './Microsoft_AspNetCore_Mvc';
import * as System from './System';

export type Mutation = {
  '/api/user/create': {
    POST: [
      ___.CreateUserResponse,
      ___.CreateUserResponse,
      (
        | Microsoft_AspNetCore_Mvc.ProblemDetails
        | Microsoft_AspNetCore_Mvc.ProblemDetails
        | Microsoft_AspNetCore_Mvc.ProblemDetails
      ),
    ];
  };
  '/api/user/get-name': {
    GET: [System.String, System.String];
  };
};
