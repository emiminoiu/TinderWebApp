using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;           
        }
 
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);     
        }

        public void SaveAll()
        {
             _context.SaveChanges();       
        }
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if(userParams.minAge != 18 || userParams.maxAge != 99) {

                var minDob = DateTime.Today.AddYears(-userParams.maxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.minAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if(userParams.OrderBy != null) {
                if(userParams.OrderBy.Equals("LastActive")) {
                    users = users.OrderByDescending(u => u.LastActive);
                }
                if(userParams.OrderBy.Equals("Created")) {
                    users = users.OrderByDescending(u => u.Created);
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }
        
         public async Task<Photo> GetMainPhoto(int userId)
         {
             var mainPhoto = await _context.Photos
             .Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.isMain);
             return mainPhoto;
         }

    }
}