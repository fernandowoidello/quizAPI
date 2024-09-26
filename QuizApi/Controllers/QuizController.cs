using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApi.Data;
using QuizApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.IO;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly QuizContext _context;
        private readonly IMapper _mapper;

        public QuizController(QuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Quiz/questions
        // Obtém uma lista de todas as perguntas, incluindo suas opções, e as retorna como QuestionDto.

        [HttpGet("questions")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
        {
            //todas as perguntas, incluindo as opções relacionadas
            var questions = await _context.Questions
                                          .Include(q => q.Options)
                                          .OrderBy(q => q.Id) // Ordena as perguntas pelo Id (pode ser alterado conforme necessidade)
                                          .ToListAsync();

            // Inicializa uma lista para armazenar os Dtos com numeração
            var numberedQuestionDtos = new List<QuestionDto>();

            // Mapeia as perguntas para QuestionDto e adiciona a numeração
            for (int i = 0; i < questions.Count; i++)
            {
                var questionDto = _mapper.Map<QuestionDto>(questions[i]);

                // Adiciona a numeração à pergunta (pode-se criar um novo campo em QuestionDto, ex: "QuestionNumber")
                questionDto.QuestionNumber = i + 1; // Numerando a partir de 1

                numberedQuestionDtos.Add(questionDto);
            }

            // Retorna a lista de QuestionDto com a numeração
            return Ok(numberedQuestionDtos);
        }
        //[HttpGet("questions")]
        //public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
        //{
        //    var questions = await _context.Questions.Include(q => q.Options).ToListAsync();
        //    var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
        //    return Ok(questionDtos);
        //}

        // GET: api/Quiz/questions/{id}
        // Obtém uma pergunta específica pelo ID, incluindo suas opções, e a retorna como QuestionDto.
        [HttpGet("questions/{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var questionDto = _mapper.Map<QuestionDto>(question);
            return Ok(questionDto);
        }

        // PUT: api/Quiz/questions/{id}
        // Atualiza uma pergunta existente com os dados fornecidos no QuestionDto.
        [HttpPut("questions/{id}")]
        public async Task<IActionResult> PutQuestion(int id, QuestionDto questionDto)
        {
            var existingQuestion = await _context.Questions.FindAsync(id);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            _mapper.Map(questionDto, existingQuestion);
            _context.Entry(existingQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Quiz/questions
        // Cria uma nova pergunta com os dados fornecidos no QuestionDto e a adiciona ao banco de dados.
        [HttpPost("questions")]
        public async Task<ActionResult<Question>> PostQuestion(QuestionDto questionDto)
        {

            var question = _mapper.Map<Question>(questionDto);
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        // POST: api/Quiz/questionsLista
        [HttpPost("questionlist")]
        public async Task<ActionResult<bool>> PostQuestion(List<QuestionDto> questionDto)
        {
            var lista = new List<Question>();
            foreach (var question in questionDto)
                lista.Add(_mapper.Map<Question>(question));

            _context.Questions.AddRange(lista);//aDDRANGE, PASSAR UMA LISTA NO BANCO.
            await _context.SaveChangesAsync();

            return Ok();
        }


        // DELETE: api/Quiz/questions/{id}
        // Remove uma pergunta existente pelo ID.
        [HttpDelete("questions/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();


            //variável local que armazena a contagem de perguntas retornada por await _context.Questions.CountAsync().
            // Retorna a contagem de perguntas atualizada
            var totalQuestions = await _context.Questions.CountAsync();
            return Ok(new { TotalQuestions = totalQuestions });

            //return NoContent();
        }

        // GET: api/Quiz/search
        // Pesquisa perguntas que contêm o texto fornecido e retorna uma lista de QuestionDto.
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByText([FromQuery] string text)
        {
            var questions = await _context.Questions
                                           .Where(q => q.Text.Contains(text))
                                           .Include(q => q.Options)
                                           .ToListAsync();
            var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

            //var questions2 = await _context.Questions
            //                   .Where(q => q.Text.Contains(text))
            //                   .Include(q => q.Options)
            //                   .Select(e => new QuestionDto
            //                   {
            //                        QuestionText = e.Text,
            //                        CorrectAnswer = e.CorrectAnswer,
            //                        Answers = e.Options.Select(o => new OptionDto
            //                        {
            //                            Text = o.Text,
            //                            IsCorrect = e.CorrectAnswer == o.Text
            //                        }).ToList()
            //                   })
            //                   .ToListAsync();

            if (questionDtos == null || !questionDtos.Any())
            {
                return NotFound();
            }

            return Ok(questionDtos);
        }

        // GET: api/Quiz/questions/quantity
        // Retorna a quantidade total de perguntas no banco de dados.
        [HttpGet("questions/quantity")]
        public async Task<ActionResult<int>> GetQuestionsQuantity()
        {
            var count = await _context.Questions.CountAsync();
            return Ok(new { TotalQuestions = count });
        }

        // GET: api/Quiz/questions/json
        // Retorna o conteúdo do arquivo JSON localizado em "Data/questions.json".
        [HttpGet("questions/json")]
        public IActionResult GetQuestionsJson()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "questions.json");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("O arquivo JSON não foi encontrado.");
            }

            var jsonContent = System.IO.File.ReadAllText(filePath);
            return Content(jsonContent, "application/json");
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}




// Remover se não forem usados
//public class Answer2
//{
//    public string answerText { get; set; }
//    public bool isCorrect { get; set; }
//}

//public class Questions2
//{
//    public string questionText { get; set; }
//    public string correctAnswer { get; set; }
//    public List<Answer2> answers { get; set; }
//}
//[HttpGet("questions2")]
//public async Task<ActionResult<IEnumerable<Question>>> GetQuestions2()
//{
//    var teste = JsonConvert.DeserializeObject<List<Questions2>>("[\r\n  {\r\n    " +
//        "\"questionText\": \"Qual é a capital do Brasil?\",\r\n    " +
//        "\"correctAnswer\": \"Brasília\",\r\n    \"answers\": [\r\n      " +
//        "{ \"answerText\": \"São Paulo\", \"isCorrect\": false },\r\n      " +
//        "{ \"answerText\": \"Rio de Janeiro\", \"isCorrect\": false },\r\n      " +
//        "{ \"answerText\": \"Brasília\", \"isCorrect\": true },\r\n     " +
//        " { \"answerText\": \"Salvador\", \"isCorrect\": false }\r\n    ]\r\n  },\r\n  {\r\n    \"questionText\": \"Quantos continentes existem no planeta Terra?\",\r\n    \"correctAnswer\": \"7\",\r\n    \"answers\": [\r\n      { \"answerText\": \"5\", \"isCorrect\": false },\r\n      { \"answerText\": \"6\", \"isCorrect\": false },\r\n      { \"answerText\": \"7\", \"isCorrect\": true },\r\n      { \"answerText\": \"8\", \"isCorrect\": false }\r\n    ]\r\n  },\r\n  {\r\n    \"questionText\": \"Qual é o menor planeta do sistema solar?\",\r\n    \"correctAnswer\": \"Mercúrio\",\r\n    \"answers\": [\r\n      { \"answerText\": \"Vênus\", \"isCorrect\": false },\r\n      { \"answerText\": \"Marte\", \"isCorrect\": false },\r\n      { \"answerText\": \"Mercúrio\", \"isCorrect\": true },\r\n      { \"answerText\": \"Netuno\", \"isCorrect\": false }\r\n    ]\r\n  },\r\n  {\r\n    \"questionText\": \"O que é a fotossíntese?\",\r\n    \"correctAnswer\": \"Processo pelo qual as plantas produzem seu próprio alimento\",\r\n    \"answers\": [\r\n      { \"answerText\": \"Processo de digestão das plantas\", \"isCorrect\": false },\r\n      { \"answerText\": \"Processo de respiração das plantas\", \"isCorrect\": false },\r\n      { \"answerText\": \"Processo pelo qual as plantas produzem seu próprio alimento\", \"isCorrect\": true },\r\n      { \"answerText\": \"Processo de crescimento das plantas\", \"isCorrect\": false }\r\n    ]\r\n  },\r\n  {\r\n    \"questionText\": \"Quem escreveu 'Dom Casmurro'?\",\r\n    \"correctAnswer\": \"Machado de Assis\",\r\n    \"answers\": [\r\n      { \"answerText\": \"José de Alencar\", \"isCorrect\": false },\r\n      { \"answerText\": \"Machado de Assis\", \"isCorrect\": true },\r\n      { \"answerText\": \"Monteiro Lobato\", \"isCorrect\": false },\r\n      { \"answerText\": \"Carlos Drummond de Andrade\", \"isCorrect\": false }\r\n    ]\r\n  },\r\n  {\r\n    \"questionText\": \"Qual é o símbolo químico da água?\",\r\n    \"correctAnswer\": \"H2O\",\r\n    \"answers\": [\r\n      { \"answerText\": \"O2\", \"isCorrect\": false },\r\n      { \"answerText\": \"H2O\", \"isCorrect\": true },\r\n      { \"answerText\": \"CO2\", \"isCorrect\": false },\r\n      { \"answerText\": \"NaCl\", \"isCorrect\": false }\r\n    ]\r\n  }\r\n]");
//    return await _context.Questions.Include(q => q.Options).ToListAsync();
//}


