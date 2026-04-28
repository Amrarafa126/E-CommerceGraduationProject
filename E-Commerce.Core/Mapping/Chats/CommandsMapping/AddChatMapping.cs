using AutoMapper;
using E_Commerce.Core.Features.Chats;
using E_Commerce.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Mapping.Chats
{
    public partial class ChatProfile 
    {
        public void AddMappingChat()
        {
            CreateMap<Conversation, ConversationDto>()
          .ForMember(d => d.BuyerName, o => o.MapFrom(s => s.Buyer != null ? s.Buyer.FullName : ""))
          .ForMember(d => d.BuyerEmail, o => o.MapFrom(s => s.Buyer != null ? s.Buyer.Email : null))
          .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.CompanyName : ""))
          .ForMember(d => d.CompanyLogoUrl, o => o.MapFrom(s => s.Company != null ? s.Company.LogoUrl : null))
          .ForMember(d => d.UnreadCount, o => o.Ignore())
          .ForMember(d => d.LastMessage, o => o.Ignore());

            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender != null ? s.Sender.FullName : ""))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));
        }
    }
}
