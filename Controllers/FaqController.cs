using ChatbotFAQApi.Models;
using ChatbotFAQApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatbotFAQApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly FaqService _faqService;
        private readonly ChatSessionService _chatSessionService;

        public FaqController(FaqService faqService, ChatSessionService chatSessionService)
        {
            _faqService = faqService;
            _chatSessionService = chatSessionService;
        }

        // 1. Get all FAQs
        //[HttpResponseType]
        [HttpGet]
        public async Task<List<FaqItem>> Get() =>
            await _faqService.GetAsync();

        // 2. Get FAQ by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var faq = await _faqService.GetByIdAsync(id);
            if (faq == null)
            {
                return NotFound();
            }
            return Ok(faq);
        }

        // 3. Post single FAQ
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FaqItem faq)
        {
            await _faqService.CreateAsync(faq);
            return Ok(faq);
        }

        // 4. Post FAQs in bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> PostBulkFaqs([FromBody] FaqBulkRequest request)
        {
            if (request?.Items == null || !request.Items.Any())
                return BadRequest("No FAQ data provided.");
            await _faqService.CreateManyAsync(request.Items);
            return Ok(new { message = "FAQs saved successfully" });
        }

        // 5. Update FAQ by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFaqs(string id, [FromBody] FaqItem updatedFaq)
        {
            var existingFaq = await _faqService.GetByIdAsync(id);
            if (existingFaq == null)
            {
                return NotFound();
            }
            updatedFaq.Id = id; // Ensure the ID is set
            await _faqService.UpdateAsync(id, updatedFaq);
            return Ok(updatedFaq);
        }

        // 6. Delete FAQ by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingFaq = await _faqService.GetByIdAsync(id);
            if (existingFaq == null)
            {
                return NotFound();
            }
            await _faqService.DeleteAsync(id);
            return NoContent(); // 204
        }

        // 6a. Delete all FAQs
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAll()
        {
            await _faqService.DeleteAllAsync();
            return NoContent(); // 204
        }

        // 7. Start chat (no session)
        [HttpPost("chat/start")]
        public async Task<IActionResult> StartChat([FromBody] ChatRequest request)
        {
            string sessionId = Guid.NewGuid().ToString();
            var session = new Models.ChatSession { SessionId = sessionId };
            session.Messages.Add(new Models.ChatMessage
            {
                Sender = "user",
                Text = request.Message,
                Timestamp = DateTime.UtcNow
            });
            var faqs = await _faqService.GetAsync();
            var matched = faqs.FirstOrDefault(f => f.Query.Equals(request.Message, StringComparison.OrdinalIgnoreCase));
            string reply;
            List<string> options = new List<string>();
            if (matched != null)
            {
                // Check if user message matches an optionText
                var matchedOption = matched.Options?.FirstOrDefault(opt => opt.OptionText.Equals(request.Message, StringComparison.OrdinalIgnoreCase));
                if (matchedOption != null)
                {
                    reply = matchedOption.Response;
                }
                else
                {
                    reply = matched.Response;
                    options = matched.Options?.Select(opt => opt.OptionText).ToList() ?? new List<string>();
                }
            }
            else
            {
                reply = "Sorry, I don't have an answer for that.";
            }
            session.Messages.Add(new Models.ChatMessage
            {
                Sender = "bot",
                Text = reply,
                Timestamp = DateTime.UtcNow
            });
            await _chatSessionService.CreateAsync(session);
            return Ok(new { sessionId, reply, options });
        }

        // 8. Continue chat (by session)
        [HttpPost("chat/{sessionId}")]
        public async Task<IActionResult> ContinueChat(string sessionId, [FromBody] ChatRequest request)
        {
            var session = await _chatSessionService.GetBySessionIdAsync(sessionId);
            if (session == null)
                return NotFound(new { message = "Session not found." });
            session.Messages.Add(new Models.ChatMessage
            {
                Sender = "user",
                Text = request.Message,
                Timestamp = DateTime.UtcNow
            });
            var faqs = await _faqService.GetAsync();
            var matched = faqs.FirstOrDefault(f => f.Query.Equals(request.Message, StringComparison.OrdinalIgnoreCase));
            string reply;
            List<string> options = new List<string>();
            if (matched != null)
            {
                // Check if user message matches an optionText
                var matchedOption = matched.Options?.FirstOrDefault(opt => opt.OptionText.Equals(request.Message, StringComparison.OrdinalIgnoreCase));
                if (matchedOption != null)
                {
                    reply = matchedOption.Response;
                }
                else
                {
                    reply = matched.Response;
                    options = matched.Options?.Select(opt => opt.OptionText).ToList() ?? new List<string>();
                }
            }
            else
            {
                // Check all FAQs for a matching optionText
                var matchedOption = faqs.SelectMany(f => f.Options ?? new List<FaqOption>())
                    .FirstOrDefault(opt => opt.OptionText.Equals(request.Message, StringComparison.OrdinalIgnoreCase));
                if (matchedOption != null)
                {
                    reply = matchedOption.Response;
                }
                else
                {
                    reply = "Sorry, I don't have an answer for that.";
                }
            }
            session.Messages.Add(new Models.ChatMessage
            {
                Sender = "bot",
                Text = reply,
                Timestamp = DateTime.UtcNow
            });
            await _chatSessionService.UpdateAsync(sessionId, session);
            return Ok(new { sessionId, reply, options });
        }

        // 9. Get chat history (by session)
        [HttpGet("chat/{sessionId}")]
        public async Task<IActionResult> GetChatHistory(string sessionId)
        {
            var session = await _chatSessionService.GetBySessionIdAsync(sessionId);
            if (session == null)
                return NotFound();
            return Ok(session);
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
