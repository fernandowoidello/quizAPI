using AutoMapper;
using QuizApi.Models;
using System.Linq;

namespace QuizApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeia Question para QuestionDto
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Text))  // Mapeia 'Text' para 'QuestionText'
                .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.CorrectAnswer)) // Mapeia 'CorrectAnswer' para 'CorrectAnswer'
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Options.Select(o => new OptionDto
                {
                    Text = o.Text,
                    //IsCorrect = o.Text == src.CorrectAnswer
                }).ToList())); // Mapeia 'Options' para 'OptionDto'

            // Mapeia Option para OptionDto
            CreateMap<Option, OptionDto>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text));  // Mapeia 'Text' para 'Text'
                //.ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.Text == src.Question.CorrectAnswer)); // Determina se é a resposta correta

            // Mapeia QuestionDto para Question
            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.QuestionText))  // Mapeia 'QuestionText' para 'Text'
                .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.CorrectAnswer)) // Mapeia 'CorrectAnswer' para 'CorrectAnswer'
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Answers.Select(answer => new Option
                {
                    Text = answer.Text
                }).ToList())); // Mapeia 'OptionDto' para 'Options'

            // Mapeia OptionDto para Option
            CreateMap<OptionDto, Option>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))  // Mapeia 'Text' para 'Text'
                .ForMember(dest => dest.Question, opt => opt.Ignore()); // Ignora 'Question' na criação de uma nova opção
        }
    }
}
