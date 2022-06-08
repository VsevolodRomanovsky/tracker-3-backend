using Microsoft.EntityFrameworkCore;
using Tracker.AuthenticationService.Data.Models;
using Tracker.AuthenticationService.Dto;

namespace Tracker.AuthenticationService.Application
{
    public class DbService: IDbService
    {
        private readonly TestDataContext _context;

        public DbService(TestDataContext context)
        {
            _context = context;
        }

        public async Task<WebSecurityUsersExtended> GetWebSecurityUsersExtended(LoginDto loginDto)
        {
            var res = await _context.WebSecurityUsersExtendeds.Where(t => t.Userid == loginDto.Login).FirstOrDefaultAsync();
            return res;
        }

        public async Task UpdateWebSecurityUsersExtended(LoginDto loginDto)
        {
            var result = await _context.WebSecurityUsersExtendeds.SingleOrDefaultAsync(b => b.Userid == loginDto.Login);
            if (result != null)
            {
                result.HasNewAccount = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
