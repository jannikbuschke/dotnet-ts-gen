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
import * as ___ from './___';
import * as IntegrationTests from './IntegrationTests';

export type Api = {
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
    POST: [Microsoft_FSharp_Core.Unit, Microsoft_AspNetCore_Mvc.ActionResult];
    GET: [Microsoft_FSharp_Core.Unit, System.String];
  };
  '/api/post': {
    POST: [Microsoft_FSharp_Core.Unit, Microsoft_AspNetCore_Mvc.ActionResult];
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

import {
  DataTag,
  queryOptions,
  UndefinedInitialDataOptions,
  useQuery,
  UseQueryOptions,
  UseMutationOptions,
  mutationOptions,
} from '@tanstack/react-query';
import { FSharpResult_Case_Error } from './Microsoft_FSharp_Core';

type PathsWithGET<T extends Record<string, any>> = {
  [K in keyof T]: 'GET' extends keyof T[K] ? K : never;
}[keyof T];

type PathsWithPost<T extends Record<string, any>> = {
  [K in keyof T]: 'POST' extends keyof T[K] ? K : never;
}[keyof T];

type LastSegment<Path extends string> = Path extends `${string}/${infer Tail}`
  ? Tail extends `${infer _}/${infer Last}`
    ? LastSegment<Tail>
    : Tail
  : Path;

// Filter paths where last segment starts with 'get-'
type PathsWithGetLastSegment<T extends Record<string, any>> = {
  [K in keyof T]: LastSegment<K & string> extends `get-${string}` ? K : never;
}[keyof T];

// Filter paths where last segment starts with 'query'
type PathsWithQueryLastSegment<T extends Record<string, any>> = {
  [K in keyof T]: LastSegment<K & string> extends `query${string}` ? K : never;
}[keyof T];

type Fetch = typeof fetch;

const postFn = async <TInput, TResult>(
  fetch: Fetch,
  url: string,
  input: TInput,
  info?: Omit<RequestInit, 'method' | 'body'>
) => {
  const headers = new Headers(info?.headers);
  headers.set('Content-Type', 'application/json');
  const r = await fetch(url, {
    ...info,
    headers,
    body: JSON.stringify(input),
    method: 'POST',
  });
  const content: TResult = await (r.headers
    .get('Content-Type')
    ?.includes('application/json')
    ? r.json()
    : r.text());
  if (r.ok) {
    return content;
  } else {
    throw content;
  }
};

export type PostInfo = Omit<RequestInit, 'method' | 'body'>;

export type GetInfo = Omit<RequestInit, 'method' | 'body'>;

function toQueryString(obj: any): string {
  const str = [];
  for (const p in obj)
    if (obj.hasOwnProperty(p)) {
      const v = obj[p];
      if (typeof v === 'object') {
        str.push(encodeURIComponent(p) + '.' + toQueryString(v));
      } else {
        str.push(encodeURIComponent(p) + '=' + encodeURIComponent(obj[p]));
      }
    }
  return str.join('&');
}

const getFn = async <TInput, Response>(
  fetch: Fetch,
  url: string,
  input: TInput,
  info?: PostInfo,
  requireJson?: boolean
) => {
  const headers = new Headers(info?.headers);
  const querystring = toQueryString(input);
  const query = querystring ? `?${querystring}` : '';
  const fullUrl = `${url}${query}`;
  const response = await fetch(fullUrl, {
    ...info,
    headers,
    method: 'GET',
  });
  const contentType = response.headers.get('Content-Type');
  const isJson =
    contentType?.includes('application/json') || contentType?.includes('json');
  if ((requireJson === undefined || requireJson === true) && !isJson) {
    throw new Error(
      `Expected json response but got content type '${contentType}'`
    );
  }
  const content: Response = isJson
    ? await response.json()
    : await response.text();
  if (response.status === 204) {
    throw new Error('No content returned by server');
  }
  if (response.ok) {
    return content;
  } else {
    throw new Error(content as any);
  }
};

export function query<
  const Path extends (
    | PathsWithGetLastSegment<Api>
    | PathsWithQueryLastSegment<Api>
  ) &
    PathsWithPost<Api>,
>({
  url,
  request,
  info,
  options,
}: {
  url: Path;
  request: Api[Path]['POST'][0];
  info?: PostInfo;
  options?: Omit<
    UndefinedInitialDataOptions<
      Api[Path]['POST'][1],
      Error,
      Api[Path]['POST'][1],
      [Path, Api[Path]['POST'][0]]
    >,
    'queryKey' | 'queryFn'
  >;
}) {
  return queryOptions({
    queryKey: [url, request],
    queryFn: () =>
      postFn<Api[Path]['POST'][0], Api[Path]['POST'][1]>(
        fetch,
        url,
        request,
        info
      ),
    ...options,
  });
}

export function queryGet<const Path extends PathsWithGET<Api>>({
  url,
  request,
  info,
  options,
}: {
  url: Path;
  request: Api[Path]['GET'][0];
  info?: GetInfo;
  options?: Omit<
    UndefinedInitialDataOptions<
      Api[Path]['GET'][1],
      Error,
      Api[Path]['GET'][1],
      (string | Api[Path]['GET'][0] | undefined)[]
    >,
    'queryKey' | 'queryFn'
  >;
}) {
  return queryOptions({
    queryKey: [url, request],
    queryFn: () =>
      getFn<Api[Path]['GET'][0], Api[Path]['GET'][1]>(
        fetch,
        url,
        request,
        info
      ),
    ...options,
  });
}

export function mutate<const Path extends PathsWithPost<Api>>(path: Path) {
  return mutationOptions<Api[Path]['POST'][1], Error, Api[Path]['POST'][0]>({
    mutationKey: [path],
    mutationFn: (request) =>
      postFn<Api[Path]['POST'][0], Api[Path]['POST'][1]>(
        fetch,
        path,
        request
        // info,
      ),
    // ...options,
  });
}

// export type QueryOptions<TInput, TOutput> = Omit<
//   UndefinedInitialDataOptions<
//     TOutput,
//     Error,
//     TOutput,
//     (string | TInput | undefined)[]
//   >,
//   'queryKey' | 'queryFn'
// >;

export function postQueryOption<const Path extends PathsWithPost<Api>>({
  url,
  request: input,
  info,
  options,
}: {
  url: Path;
  request: Api[Path]['POST'][0];
  info?: GetInfo;
  options?: Omit<
    UndefinedInitialDataOptions<
      Api[Path]['POST'][1],
      Error,
      Api[Path]['POST'][1],
      (string | Api[Path]['POST'][0] | undefined)[]
    >,
    'queryKey' | 'queryFn'
  >;
}) {
  return queryOptions({
    queryKey: [url, input],
    queryFn: () =>
      postFn<Api[Path]['POST'][0], Api[Path]['POST'][1]>(
        fetch,
        url,
        input,
        info
      ),
    ...options,
  });
}
