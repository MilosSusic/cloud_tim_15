using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class RegisterScaleCommandHandler : IRequestHandler<RegisterScaleCommand, Result<Guid>>
    {
        private readonly ISmartScaleRepository _smartScaleRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public RegisterScaleCommandHandler(
            ISmartScaleRepository smartScaleRepository,
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            ICurrentUserService currentUserService)
        {
            _smartScaleRepository = smartScaleRepository;
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(RegisterScaleCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<Guid>.Failure("Invalid user id.");

            var hive = await _hiveRepository.GetByIdAsync(request.HiveId, cancellationToken);
            if (hive is null)
                return Result<Guid>.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId.Value)
                return Result<Guid>.Failure("You do not have access to this hive.");

            var isUnique = await _smartScaleRepository.IsSerialNumberUniqueAsync(request.SerialNumber, cancellationToken);
            if (!isUnique)
                return Result<Guid>.Failure("A scale with this serial number is already registered.");

            SmartScale smartScale;
            try
            {
                smartScale = SmartScale.Create(request.HiveId, request.SerialNumber);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await _smartScaleRepository.AddAsync(smartScale, cancellationToken);
            await _smartScaleRepository.SaveChangesAsync(cancellationToken);

            StartConsoleSimulator(request.SerialNumber);

            return Result<Guid>.Success(smartScale.Id);
        }
        private void StartConsoleSimulator(string serialNumber)
        {
            try
            {
                string apiBaseDir = AppContext.BaseDirectory;

                string simulatorPath = Path.GetFullPath(Path.Combine(apiBaseDir,
                    "../../../../SmartApiary.ScaleSimulator/bin/Debug/net8.0/SmartApiary.ScaleSimulator.exe"));

                if (!File.Exists(simulatorPath))
                {
                    Console.WriteLine("FILE PATH DOESN'T EXIST");
                    Console.WriteLine("==================================================");
                    Console.WriteLine($"[DEBUG] Simulator NOT found at: {simulatorPath}");
                    Console.WriteLine($"[DEBUG] Web API Base Dir was: {apiBaseDir}");
                    Console.WriteLine("==================================================");
                    return;
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = simulatorPath,
                    Arguments = serialNumber,
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                };

                System.Diagnostics.Process.Start(startInfo);
            }
            catch
            {
                Console.WriteLine("SOMETHING WENT WRONG WHILE STARTING SIMULATOR");
            }
        }
    }
}
