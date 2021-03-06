﻿using Engine.Exceptions;
using Engine.Model.Client;
using Engine.Model.Entities;
using Engine.Network.Connections;
using Engine.Plugins;
using System.Collections.Generic;
using System.Security;

namespace Engine.API.ClientCommands
{
  public abstract class ClientCommand : 
    CrossDomainObject, 
    ICommand<ClientCommandArgs>
  {
    protected virtual bool IsPeerCommand
    {
      [SecuritySafeCritical]
      get { return false; }
    }

    public abstract long Id
    {
      [SecuritySafeCritical]
      get;
    }
  
    [SecuritySafeCritical]
    public void Run(ClientCommandArgs args)
    {
      if (IsPeerCommand)
      {
        if (args.PeerConnectionId == null)
          throw new ModelException(ErrorCode.IllegalInvoker, string.Format("Command cannot be runned from server package. {0}", GetType().FullName));
      }
      else
      {
        if (args.PeerConnectionId != null)
          throw new ModelException(ErrorCode.IllegalInvoker, string.Format("Command cannot be runned from peer package. {0}", GetType().FullName));
      }

      OnRun(args);
    }

    [SecuritySafeCritical]
    protected abstract void OnRun(ClientCommandArgs args);
  }

  public abstract class ClientCommand<TContent> : ClientCommand
  {
    [SecuritySafeCritical]
    protected sealed override void OnRun(ClientCommandArgs args)
    {
      var package = args.Package as IPackage<TContent>;
      if (package == null)
        throw new ModelException(ErrorCode.WrongContentType);

      OnRun(package.Content, args);
    }

    [SecuritySafeCritical]
    protected abstract void OnRun(TContent content, ClientCommandArgs args);

    #region Helpers
    /// <summary>
    /// Обновляет пользователей в модели.
    /// </summary>
    /// <param name="client">Клиентская модель.</param>
    /// <param name="users">Пользователи.</param>
    [SecuritySafeCritical]
    protected void UpdateUsers(ClientContext client, List<User> users)
    {
      foreach (var user in users)
        client.Users[user.Nick] = user;
    }
    #endregion
  }
}
