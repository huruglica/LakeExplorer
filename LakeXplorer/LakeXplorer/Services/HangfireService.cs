using LakeXplorer.Services.ISerices;

namespace LakeXplorer.Services
{
    public class HangfireService : IhangfireService
    {
        private readonly ILakeSightingService _lakeSightingService;

        public HangfireService(ILakeSightingService lakeSightingService)
        {
            _lakeSightingService = lakeSightingService;
        }

        public async Task DailyFunFact()
        {
            await _lakeSightingService.DailyFunFact();
        }
    }
}
