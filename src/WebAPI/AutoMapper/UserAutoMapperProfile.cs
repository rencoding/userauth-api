using System.Collections.Generic;
using AutoMapper;
using DbUser = GenesisTest.DAL.Models.User;
using DbUserTelephone = GenesisTest.DAL.Models.UserTelephone;
using DomainUser = GenesisTest.WebAPI.DomainEntities.User;

namespace GenesisTest.WebAPI.AutoMapper
{
    public class UserAutoMapperProfile : Profile
    {
        public UserAutoMapperProfile()
        {
            MapTelephonesToDbModel();
            MapDomainUserToDbModel();
        }

        private void MapDomainUserToDbModel()
        {
            CreateMap<DomainUser, DbUser>()
                .ForMember(u => u.UserTelephones, s => s.MapFrom(src=> src.TelephoneNumbers));
        }

        private void  MapTelephonesToDbModel()
        {
            CreateMap<IEnumerable<string>, IEnumerable<DbUserTelephone>>().ConvertUsing(new TelephoneTypeConverter());
        }

        public class TelephoneTypeConverter : ITypeConverter<IEnumerable<string>, IEnumerable<DbUserTelephone>>
        {
            public IEnumerable<DbUserTelephone> Convert(IEnumerable<string> source, IEnumerable<DbUserTelephone> destination, ResolutionContext context)
            {
                List<DbUserTelephone> dest = new List<DbUserTelephone>();

                foreach ( var t in source)
                {
                    dest.Add(new DbUserTelephone() { TelephoneNumber = t });
                }

                return dest;
            }
        }


    }
}
