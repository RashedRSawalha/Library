//using LibraryManagementDomain.Entities;
//using LibraryManagementDomain.Mapping;
//using LibraryManagementDomain.Models;
//using LibraryManagementApplication;
//using Persistence.Interface;
//using LibraryManagementAPI.UnitOfWork;
//using MediatR;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http.HttpResults;

//namespace LibraryManagementAPI.Handlers.Authors
//{
//    public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, AuthorModel>
//    {
//        private readonly IRepository<Author> _authorRepository;
//        private readonly IUnitOfWork _unitOfWork;

//        public AddAuthorCommandHandler(IRepository<Author> authorRepository, IUnitOfWork unitOfWork)
//        {
//            _authorRepository = authorRepository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<AuthorModel> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
//        {
//            var entity = request.Model.ToEntity();

//            await _authorRepository.AddAsync(entity);
//            await _unitOfWork.SaveAndCommitAsync();

//            return entity;
//        }
//    }
//}
