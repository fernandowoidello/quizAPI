using System.Collections.Generic;

namespace QuizApi.Models
{
    // Representa uma pergunta com opções
    public class Question
    {

      
        public int Id { get; set; } // Identificador único da pergunta

        public string Text { get; set; } // Texto da pergunta

        public string CorrectAnswer { get; set; } // Resposta correta

        public ICollection<Option> Options { get; set; } // Opções de resposta
    }

    // Representa uma opção de resposta para uma pergunta
    public class Option
    {
        public int Id { get; set; } // Identificador único da opção
        public string Text { get; set; } // Texto da opção
        public int QuestionId { get; set; } // ID da pergunta associada
        public Question Question { get; set; } // Referência à pergunta associada
    }

   
    public class QuestionDto
    {

        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public List<OptionDto> Answers { get; set; }
    }

    // DTO para representar uma opção de resposta
    public class OptionDto
    {
        //public int QuestionNumber { get; set; } // identificar o numero da pergunta
        public string Text { get; set; } // Texto da opção
        //public bool IsCorrect { get; set; } // Indica se a opção é a resposta correta
    }
}


//legenda
////Question: Representa uma pergunta com um texto, a resposta correta e uma coleção de opções.
////Option: Representa uma opção de resposta associada a uma pergunta, com texto e um identificador da pergunta.
////QuestionDto: DTO para retornar uma pergunta no formato JSON, com a lista de opções representadas como OptionDto.
////OptionDto: DTO para representar uma opção de resposta, com texto e um indicador de se é a resposta correta.
//}

//o front-end poderá entender a quantidade de perguntas na api

//o que acontece se eu remover uma pergunta ok
//e como o front vai entender a quantidade de perguntas a partir da exclusao
//como mapear o numero da pergunta com a quantidade de perguntas a partir da exlusao. indeneptnede do id da pergunta.