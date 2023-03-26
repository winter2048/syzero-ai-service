﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SyZero;

namespace SyZero.OpenAI.Web.Filter
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class AppExceptionFilter : IExceptionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnException(ExceptionContext context)
        {

            var _Exception = context.Exception;
            if (_Exception is SyMessageException _Error)
            {
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = 200;

                if (_Error.Model.code == (int)SyMessageBoxStatus.Custom)
                    context.Result = new JsonResult(_Error.Model.msg);
                else
                    context.Result = new JsonResult(_Error.Model);
            }
            else
            {
                // Tools.Log.Write(_Exception, context.HttpContext.Connection.RemoteIpAddress.ToString());//nlog 写入日志到 txt
                var _MessageBoxModel = new SyMessageBoxModel($"服务端出现异常![异常消息：{_Exception.Message}]", SyMessageBoxStatus.Abnormal);
                context.Result = new JsonResult(_MessageBoxModel);
            }
        }
    }



}



