namespace TinyTypeGen.Log

[<AutoOpen>]
module Types =
  let boxunbox a = a |> (box >> unbox)
  open System

  type LogLevel =
    | Trace = 0
    | Debug = 1
    | Info = 2
    | Warn = 3
    | Error = 4
    | Fatal = 5

  /// An optional message thunk.
  ///
  /// - If <see cref="T:Microsoft.FSharp.Core.Option<_>.None">None</see> is provided, this typically signals to the logger to do a isEnabled check.
  /// - If <see cref="T:Microsoft.FSharp.Core.Option<_>.Some">Some</see> is provided, this signals the logger to log.
  type MessageThunk = (unit -> string) option

  /// The signature of a log message function
  type Logger = LogLevel -> MessageThunk -> exn option -> obj array -> bool
  type MappedContext = string -> obj -> bool -> IDisposable

  /// Type representing a Log
  [<NoEquality; NoComparison>]
  type Log =
    {
      LogLevel: LogLevel
      Message: MessageThunk
      Exception: exn option
      Parameters: obj list
      AdditionalNamedParameters: ((string * obj * bool) list)
    }

    static member StartLogLevel(logLevel: LogLevel) =
      {
        LogLevel = logLevel
        Message = None
        Exception = None
        Parameters = List.empty
        AdditionalNamedParameters = List.empty
      }

  /// An interface wrapper for a<see cref="T:FsLibLog.Types.Logger">Logger</see>. Useful when using depedency injection frameworks.
  type ILog =
    abstract member Log: Logger
    abstract member MappedContext: MappedContext

#if FABLE_COMPILER
  // Fable doesn't support System.Collections.Generic.Stack, so this implementation (from FCS)
  // is used instead.
  type Stack<'a>() =
    let mutable contents = Array.zeroCreate<'a> (2)
    let mutable count = 0

    member buf.Ensure newSize =
      let oldSize = contents.Length

      if newSize > oldSize then
        let old = contents
        contents <- Array.zeroCreate (max newSize (oldSize * 2))
        Array.blit old 0 contents 0 count

    member buf.Count = count

    member buf.Pop() =
      let item = contents.[count - 1]
      count <- count - 1
      item

    member buf.Peep() = contents.[count - 1]

    member buf.Top(n) =
      [ for x in contents.[max 0 (count - n) .. count - 1] -> x ] |> List.rev

    member buf.Push(x) =
      buf.Ensure(count + 1)
      contents.[count] <- x
      count <- count + 1

    member buf.IsEmpty = (count = 0)
#endif

  [<AutoOpen>]
  module Inner =
#if !FABLE_COMPILER
    open System.Collections.Generic
#endif

    /// <summary>
    /// DisposableStack on Dispose will call dispose on items appended to its stack in Last In First Out.
    /// </summary>
    type DisposableStack() =
      let stack = Stack<IDisposable>()

      interface IDisposable with
        member __.Dispose() =
          while stack.Count > 0 do
            stack.Pop().Dispose()

      member __.Push(item: IDisposable) = stack.Push item

      member __.Push(items: IDisposable list) = items |> List.iter stack.Push

      static member Create(items: IDisposable list) =
        let ds = new DisposableStack()
        ds.Push items
        ds

    type ILog with

      /// <summary>
      /// Logs a log
      /// </summary>
      /// <param name="log">The type representing a log message to be logged</param>
      /// <returns><see langword="true"/> if the log message was logged</returns>
      member logger.fromLog(log: Log) =
        use __ =
          log.AdditionalNamedParameters
          |> List.map (fun (key, value, destructure) -> logger.MappedContext key value destructure)
          // This stack is important, it causes us to unwind as if you have multiple uses in a row
          |> DisposableStack.Create

        log.Parameters
        |> List.toArray
        |> logger.Log log.LogLevel log.Message log.Exception

      /// <summary>
      /// Logs a fatal log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      /// <returns><see langword="true"/>  if the log message was logged</returns>
      member logger.fatal'(logConfig: Log -> Log) =
        Log.StartLogLevel LogLevel.Fatal |> logConfig |> logger.fromLog

      /// <summary>
      /// Logs a fatal log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      member logger.fatal(logConfig: Log -> Log) = logger.fatal' logConfig |> ignore

      /// <summary>
      /// Logs an error log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      /// <returns><see langword="true"/>  if the log message was logged</returns>
      member logger.error'(logConfig: Log -> Log) =
        Log.StartLogLevel LogLevel.Error |> logConfig |> logger.fromLog

      /// <summary>
      /// Logs an error log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      member logger.error(logConfig: Log -> Log) = logger.error' logConfig |> ignore

      /// <summary>
      /// Logs a warn log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      /// <returns><see langword="true"/>  if the log message was logged</returns>
      member logger.warn'(logConfig: Log -> Log) =
        Log.StartLogLevel LogLevel.Warn |> logConfig |> logger.fromLog

      /// <summary>
      /// Logs a warn log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      member logger.warn(logConfig: Log -> Log) = logger.warn' logConfig |> ignore

      /// <summary>
      /// Logs an info log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      /// <returns><see langword="true"/>  if the log message was logged</returns>
      member logger.info'(logConfig: Log -> Log) =
        Log.StartLogLevel LogLevel.Info |> logConfig |> logger.fromLog

      /// <summary>
      /// Logs an info log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      member logger.info(logConfig: Log -> Log) = logger.info' logConfig |> ignore

      /// <summary>
      /// Logs a debug log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      /// <returns><see langword="true"/>  if the log message was logged</returns>
      member logger.debug'(logConfig: Log -> Log) =
        Log.StartLogLevel LogLevel.Debug |> logConfig |> logger.fromLog

      /// <summary>
      /// Logs a debug log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      member logger.debug(logConfig: Log -> Log) = logger.debug' logConfig |> ignore

      /// <summary>
      /// Logs a trace log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      /// <returns><see langword="true"/>  if the log message was logged</returns>
      member logger.trace'(logConfig: Log -> Log) =
        Log.StartLogLevel LogLevel.Trace |> logConfig |> logger.fromLog

      /// <summary>
      /// Logs a trace log message given a log configurer.
      /// </summary>
      /// <param name="logConfig">A function to configure a log</param>
      member logger.trace(logConfig: Log -> Log) = logger.trace' logConfig |> ignore


  /// An interface for retrieving a concrete logger such as Serilog, Nlog, etc.
  type ILogProvider =
    abstract member GetLogger: string -> Logger
    abstract member OpenNestedContext: string -> IDisposable
    abstract member OpenMappedContext: string -> obj -> bool -> IDisposable

  module Log =

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with a message.
    /// </summary>
    /// <param name="message">The message to set for the log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let setMessage (message: string) (log: Log) =
      { log with Message = Some(fun () -> message) }

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with a message thunk.  Useful for "expensive" string construction scenarios.
    /// </summary>
    /// <param name="messageThunk">The function that generates a message to add to a Log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let setMessageThunk (messageThunk: unit -> string) (log: Log) =
      { log with Message = Some messageThunk }

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with a parameter.
    /// </summary>
    /// <param name="param">The value to add to the log</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let addParameter<'a when 'a: not null> (param: 'a) (log: Log) =
      { log with Parameters = List.append log.Parameters [ (boxunbox param) ] }

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with a list of parameters.
    /// </summary>
    /// <param name="params">The values to add to the log, in the form of an `obj list`.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let addParameters (``params``: obj list) (log: Log) =
      let ``params`` = ``params`` |> List.map boxunbox

      { log with Parameters = log.Parameters @ ``params`` }


    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with additional named parameters for context. This helper adds more context to a log.
    /// This DOES NOT affect the parameters set for a message template.
    /// This is the same calling OpenMappedContext right before logging.
    /// </summary>
    /// <param name="key">The key of the parameter to add to the log.</param>
    /// <param name="value">The value of the parameter to add to the log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let addContext (key: string) (value: obj) (log: Log) =
      { log with AdditionalNamedParameters = List.append log.AdditionalNamedParameters [ key, boxunbox value, false ] }

    let addOptionalContext (key: string) (value: 't option) (log: Log) =
      { log with AdditionalNamedParameters = List.append log.AdditionalNamedParameters [ key, boxunbox value, false ] }

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with additional named parameters for context. This helper adds more context to a log.
    /// This DOES NOT affect the parameters set for a message template.
    /// This is the same calling OpenMappedContext right before logging.
    /// This destructures an object rather than calling `ToString()` on it.
    /// WARNING: Destructring can be expensive.
    /// </summary>
    /// <param name="key">The key of the parameter to add to the log.</param>
    /// <param name="value">The value of the parameter to add to the log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let addContextDestructured (key: string) (value: obj) (log: Log) =
      { log with AdditionalNamedParameters = List.append log.AdditionalNamedParameters [ key, boxunbox, true ] }


    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with an <see cref="T:System.Exception">exn</see>. Handles nulls.
    /// </summary>
    /// <param name="exception">The exception to add to the log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let addException (``exception``: exn) (log: Log) =
      { log with Exception = Some ``exception`` }

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with an <see cref="T:System.Exception">exn</see>. Handles nulls.
    /// </summary>
    /// <param name="exception">The exception to add to the log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let addExn (``exception``: exn) (log: Log) = addException ``exception`` log

    /// <summary>
    /// Amends a <see cref="T:FsLibLog.Types.Log">Log</see> with a given <see cref="T:FsLibLog.Types.LogLevel">LogLevel</see>
    /// </summary>
    /// <param name="logLevel">The level to set for the log.</param>
    /// <param name="log">The log to amend.</param>
    /// <returns>The amended log.</returns>
    let setLogLevel (logLevel: LogLevel) (log: Log) = { log with LogLevel = logLevel }

/// Provides operators to make writing logs more streamlined.
module Operators =

  /// <summary>
  /// Amend a log with a message. Wrapper for <see cref="M:FsLibLog.Types.LogModule.setMessage">Log.setMessage</see>.
  /// </summary>
  /// <param name="message">The string of the base message.</param>
  /// <returns>A new Log instance with the specified message.</returns>
  let (!!!) message = Log.setMessage message

  /// <summary>
  /// Amends a log with a parameter. Wrapper for <see cref="M:FsLibLog.Types.LogModule.addParameter">Log.addParameter</see>.
  /// </summary>
  /// <param name="log">The Log to add the parameter to.</param>
  /// <param name="value">The value for the parameter.</param>
  /// <returns>The Log with the added parameter.</returns>
  let (>>!) log value = log >> Log.addParameter value

  /// <summary>
  /// Amends a Log with additional named parameters for context. This helper adds more context to a log.
  /// This DOES NOT affect the parameters set for a message template. This is the same calling OpenMappedContext right before logging.
  ///
  /// Wrapper for <see cref="M:FsLibLog.Types.LogModule.addContext">Log.addContext</see>.
  /// </summary>
  /// <param name="log">The log to add the parameter to.</param>
  /// <param name="key">The name for the parameter.</param>
  /// <param name="value">The value for the parameter.</param>
  /// <returns>The amended log with the parameter added.</returns>
  let (>>!-) log (key, value) = log >> Log.addContext key value

  /// <summary>
  /// Amends a Log with additional named parameters for context. This helper adds more context to a log. This DOES NOT affect the parameters set for a message template.
  /// This is the same calling OpenMappedContext right before logging. This destructures an object rather than calling ToString() on it. WARNING: Destructring can be expensive.
  ///
  /// Wrapper for <see cref="M:FsLibLog.Types.LogModule.addContextDestructured">Log.addContextDestructured</see>.
  /// </summary>
  /// <param name="log">The log to add the parameter to.</param>
  /// <param name="key">The name for the parameter.</param>
  /// <param name="value">The value for the parameter.</param>
  /// <returns>The amended log with the parameter added.</returns>
  let (>>!+) log (key, value) =
    log >> Log.addContextDestructured key value

  /// <summary>
  /// Amends a Log with an exn. Handles nulls.
  ///
  /// Wrapper for <see cref="M:FsLibLog.Types.LogModule.addException">Log.addException</see>.
  /// </summary>
  /// <param name="log">The log to add the parameter to.</param>
  /// <param name="e">The exception to add to the log.</param>
  /// <returns>The amended log with the parameter added.</returns>
  let (>>!!) log e = log >> Log.addException e



module LogProvider =
  open System
  open Types
  open System.Diagnostics
  open Microsoft.FSharp.Quotations.Patterns

  let mutable private currentLogProvider = None

  let private knownProviders = []

  /// Greedy search for first available LogProvider. Order of known providers matters.
  let private resolvedLogger =
    lazy
      (knownProviders
       |> Seq.tryFind (fun (isAvailable, _) -> isAvailable ())
       |> Option.map (fun (_, create) -> create ()))

  let private noopLogger _ _ _ _ = false

  let private noopDisposable =
    { new IDisposable with
        member __.Dispose() = ()
    }

  /// <summary>
  /// Allows custom override when a <c>getLogger</c> function searches for a LogProvider.
  /// </summary>
  /// <param name="logProvider">The <see cref="M:FsLibLog.Types.ILogProvider"/> to set</param>
  /// <returns></returns>
  let setLoggerProvider (logProvider: ILogProvider) = currentLogProvider <- Some logProvider

  /// <summary>
  /// Gets the currently set LogProvider or attempts to find known built in providers
  /// </summary>
  /// <returns></returns>
  let getCurrentLogProvider () =
    match currentLogProvider with
    | None -> resolvedLogger.Value
    | Some p -> Some p

  /// <summary>
  /// Opens a mapped diagnostic context.  This will allow you to set additional parameters to a log given a scope.
  /// </summary>
  /// <param name="key">The name of the property.</param>
  /// <param name="value">The value of the property.</param>
  /// <param name="destructureObjects">If true, and the value is a non-primitive, non-array type, then the value will be converted to a structure; otherwise, unknown types will be converted to scalars, which are generally stored as strings. WARNING: Destructring can be expensive.</param>
  /// <returns>An IDisposable upon disposing will remove this value from a loggers scope</returns>
  let openMappedContextDestucturable (key: string) (value: obj) (destructureObjects: bool) =
    let provider = getCurrentLogProvider ()

    match provider with
    | Some p -> p.OpenMappedContext key value destructureObjects
    | None -> noopDisposable


  /// <summary>
  /// Opens a mapped diagnostic context.  This will allow you to set additional parameters to a log given a scope. Sets destructureObjects to false.
  /// </summary>
  /// <param name="key">The name of the property.</param>
  /// <param name="value">The value of the property.</param>
  /// <returns>An IDisposable upon disposing will remove this value from a loggers scope</returns>
  let openMappedContext (key: string) (value: obj) =
    //TODO: We should try to find out if the value is a primitive
    openMappedContextDestucturable key value false


  /// <summary>
  /// Opens a nested diagnostic context.  This will allow you to set additional parameters to a log given a scope.
  /// </summary>
  /// <param name="value">The value of the property</param>
  /// <returns>An IDisposable upon disposing will remove this value from a loggers scope</returns>
  let openNestedContext (value: string) =
    let provider = getCurrentLogProvider ()

    match provider with
    | Some p -> p.OpenNestedContext value
    | None -> noopDisposable

  /// <summary>
  /// Creates a logger given a <see cref="T:System.String">string</see>. This will attempt to retrieve any loggers set with <see cref="M:FsLibLog.LogProviderModule.setLoggerProvider">Log.setLoggerProvider</see>.  It will fallback to a known list of providers.
  /// </summary>
  /// <param name="name">A name to give a logger. This can help you identify the location of where the log occurred upon reviewing the logs.</param>
  /// <returns></returns>
  let getLoggerByName (name: string) =
    let loggerProvider = getCurrentLogProvider ()

    let logFunc =
      match loggerProvider with
      | Some loggerProvider -> loggerProvider.GetLogger(name)
      | None -> noopLogger

    { new ILog with
        member x.Log = logFunc
        member x.MappedContext = openMappedContextDestucturable
    }

  /// <summary>
  /// Creates a logger given a <see cref="T:System.Type">Type</see>.  This will attempt to retrieve any loggers set with <see cref="M:FsLibLog.LogProviderModule.setLoggerProvider">Log.setLoggerProvider</see>.  It will fallback to a known list of providers.
  /// </summary>
  /// <param name="objectType">The type to generate a logger name from. </param>
  /// <returns></returns>
  let getLoggerByType (objectType: Type) = objectType |> string |> getLoggerByName

  /// <summary>
  /// Creates a logger given a <c>'a</c> type. This will attempt to retrieve any loggers set with <see cref="M:FsLibLog.LogProviderModule.setLoggerProvider">Log.setLoggerProvider</see>.  It will fallback to a known list of providers.
  /// </summary>
  /// <typeparam name="'a">The type to generate a name from.</typeparam>
  /// <returns></returns>
  let inline getLoggerFor<'a> () = getLoggerByType (typeof<'a>)
