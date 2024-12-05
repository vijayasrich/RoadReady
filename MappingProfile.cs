using AutoMapper;
using RoadReady.Models;
using RoadReady.Models.DTO;

namespace RoadReady
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for Car
            CreateMap<Car, CarDTO>().ReverseMap();  
            CreateMap<Car, CreateCarDTO>().ReverseMap();  
            CreateMap<Car, UpdateCarDTO>().ReverseMap();  

            // Mapping for CarExtra
            CreateMap<CarExtra, CarExtraDTO>().ReverseMap();  
            CreateMap<CarExtra, CreateCarExtraDTO>().ReverseMap();  
            CreateMap<CarExtra, UpdateCarExtraDTO>().ReverseMap();  

            // Mapping for Payment
            CreateMap<Payment, PaymentDTO>().ReverseMap();  
            CreateMap<CreatePaymentDTO, Payment>();  
            CreateMap<Payment, UpdatePaymentDTO>().ReverseMap();  

            // Mapping for Reservation
            CreateMap<Reservation, ReservationDTO>()
                .ForMember(dest => dest.Extras, opt => opt.MapFrom(src => src.Extras.Select(ce => new CarExtraDTO
                {
                    ExtraId = ce.ExtraId,
                    Name = ce.Name,
                    Price = (decimal)ce.Price
                }).ToList()));

            // Mapping for CreateReservationDTO to Reservation
            CreateMap<CreateReservationDTO, Reservation>()
                .ForMember(dest => dest.Extras, opt => opt.MapFrom(src =>
                    src.CarExtraIds.Select(id => new CarExtra { ExtraId = id }).ToList()));  

            CreateMap<Reservation, UpdateReservationDTO>().ReverseMap();  

            // Mapping for User
            CreateMap<User, Models.DTO.UserDTO>().ReverseMap();  
            CreateMap<User, CreateUserDTO>().ReverseMap();  
            CreateMap<User, UpdateUserDTO>().ReverseMap();  

            // Mapping for Review
            CreateMap<Review, ReviewsDTO>().ReverseMap();   
            CreateMap<Review, CreateReviewsDTO>().ReverseMap();  
            CreateMap<Review, UpdateReviewsDTO>().ReverseMap();  

            CreateMap<PasswordReset, PasswordResetDTO>().ReverseMap();
            CreateMap<ReviewRequestDTO, Review>().ReverseMap();
        }
    }
}
