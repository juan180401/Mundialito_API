using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Common;

namespace Application.Commands.Teams
{
    public class DeleteTeamCommandHandler
    {
        private readonly ITeamRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTeamCommandHandler(
            ITeamRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteTeamCommand command)
        {
            var team = await _repository.GetByIdAsync(command.Id);

            if (team is null)
                return Result.Success(); // idempotente

            _repository.Remove(team);

            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
    }
}
