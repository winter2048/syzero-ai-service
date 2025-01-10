using AutoMapper;
using Dynamitey.DynamicObjects;
using Microsoft.Extensions.AI;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SyZero.AI.Core.Chat;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.Serialization;
using SyZero.Util;

namespace SyZero.AI.Application.MapProfile
{
    public class AppMapProfile : Profile
    {
        private readonly IJsonSerialize _jsonSerialize = SyZeroUtil.GetService<IJsonSerialize>();

        public AppMapProfile()
        {
            CreateMap<Scene, SceneDto>().ForMember(des => des.Content, opt => opt.MapFrom(p => _jsonSerialize.JSONToObject<List<ChatMessageDto>>(p.Content)));
            CreateMap<SceneDto, Scene>().ForMember(des => des.Content, opt => opt.MapFrom(p => _jsonSerialize.ObjectToJSON(p.Content)));
            CreateMap<ChatMessageDto, ChatMessage>().ConvertUsing((des, opt) =>
            {
                ChatRole role;
                switch (des.Role)
                {
                    case MessageRoleEnum.User:
                        role = ChatRole.User;
                        break;
                    case MessageRoleEnum.Assistant:
                        role = ChatRole.Assistant;
                        break;
                    case MessageRoleEnum.System:
                        role = ChatRole.System;
                        break;
                    default:
                        role = ChatRole.System;
                        break;
                }
                return new ChatMessage(role, des.Content);
            });
        }
    }
}