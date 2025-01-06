using AutoMapper;
using Dynamitey.DynamicObjects;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SyZero.OpenAI.Core.Chat;
using SyZero.OpenAI.IApplication.Chat.Dto;
using SyZero.Serialization;
using SyZero.Util;

namespace SyZero.OpenAI.Application.MapProfile
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            IJsonSerialize jsonSerialize = SyZeroUtil.GetService<IJsonSerialize>();
            CreateMap<Scene, SceneDto>().ForMember(des => des.Content, opt => opt.MapFrom(p => jsonSerialize.JSONToObject<List<ChatMessageDto>>(p.Content)));
            CreateMap<SceneDto, Scene>().ForMember(des => des.Content, opt => opt.MapFrom(p => jsonSerialize.ObjectToJSON(p.Content)));
        }
    }
}