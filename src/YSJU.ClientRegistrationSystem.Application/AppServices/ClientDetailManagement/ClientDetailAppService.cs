using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YSJU.ClientRegistrationSystem.AppEntities.ClientDetails;
using YSJU.ClientRegistrationSystem.AppEntities.ProductCategories;
using YSJU.ClientRegistrationSystem.AppEntities.Products;
using YSJU.ClientRegistrationSystem.Interfaces.ClientDetailManagement;
using YSJU.ClientRegistrationSystem.Dtos.ResponseDtos;
using YSJU.ClientRegistrationSystem.Dtos.ClientDetailDtos;

namespace YSJU.ClientRegistrationSystem.AppServices.ClientDetailManagement
{
    public class ClientDetailAppService : ApplicationService, IClientDetailAppService
    {
        private readonly IRepository<ClientDetail, Guid> _clientPersonalDetailRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;

        public ClientDetailAppService(
            IRepository<ClientDetail, Guid> clientPersonalDetailRepository,
            IRepository<Product, Guid> productRepository = null,
            IRepository<ProductCategory, Guid> productCategoryRepository = null)
        {
            _clientPersonalDetailRepository = clientPersonalDetailRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<ResponseDto<ClientDetailResponseDto>> CreateClientDetailAsync(CreateClientDetailDto input)
        {
            try
            {
                Logger.LogInformation($"CreateClientDetailAsync requested by User: {CurrentUser.Id}");
                Logger.LogDebug($"CreateClientDetailAsync requested for User: {(CurrentUser.Id, input)}");

                var clientPersonalDetailQuery = await _clientPersonalDetailRepository.GetQueryableAsync();
                var productQuery = await _productRepository.GetQueryableAsync();

                var newClientPersonalDetails = new ClientDetail
                {
                    ClientId = 1,
                    FirstName = input.FirstName,
                    MiddleName = input.MiddleName,
                    LastName = input.LastName,
                    Address = input.Address,
                    PhoneNumber = input.PhoneNumber,
                    Email = input.Email,
                    ProductId = input.ProductId,
                };

                await _clientPersonalDetailRepository.InsertAsync(newClientPersonalDetails);

                var query = (from clientPersonalDetail in clientPersonalDetailQuery
                             join product in productQuery on clientPersonalDetail.ProductId equals product.Id into productLeft
                             from product in productLeft.DefaultIfEmpty()
                             where clientPersonalDetail.Id == newClientPersonalDetails.Id
                             select new ClientDetailResponseDto
                             {
                                 Id = clientPersonalDetail.Id,
                                 FirstName = clientPersonalDetail.FirstName,
                                 MiddleName = clientPersonalDetail.MiddleName,
                                 LastName = clientPersonalDetail.LastName,
                                 Address = clientPersonalDetail.Address,
                                 PhoneNumber = clientPersonalDetail.PhoneNumber,
                                 Email = clientPersonalDetail.Email,
                                 ProductId = clientPersonalDetail.ProductId,
                                 ProductName = product.Name,
                             }).FirstOrDefault();

                var response = new ResponseDto<ClientDetailResponseDto>
                {
                    Success = true,
                    Code = 200,
                    Message = "New Client Details added successfully",
                    Data = query
                };

                Logger.LogInformation($"CreateClientDetailAsync responded for User: {CurrentUser.Id}");

                return response;
            }
            catch (Exception)
            {
                Logger.LogError(nameof(CreateClientDetailAsync));
                throw;
            }
        }

        public async Task<ClientDetailResponseDto> GetClientDetailById(Guid clientPersonalDetailId)
        {
            try
            {
                Logger.LogInformation($"GetClientDetailById requested by User: {CurrentUser.Id}");
                Logger.LogDebug($"GetClientDetailById requested for User: {(CurrentUser.Id, clientPersonalDetailId)}");

                var clientPersonalDetailQuery = await _clientPersonalDetailRepository.GetQueryableAsync();
                var productQuery = await _productRepository.GetQueryableAsync();

                var clientPersonalDetail = clientPersonalDetailQuery.Where(x => x.Id == clientPersonalDetailId).FirstOrDefault()
                    ?? throw new UserFriendlyException("Client Personal Detail not found", code: "400");

                var clientPersonalDetailData = (from clientPersonalDetails in clientPersonalDetailQuery
                                                join product in productQuery on clientPersonalDetails.ProductId equals product.Id into productLeft
                                                from product in productLeft.DefaultIfEmpty()
                                                where clientPersonalDetail.Id == clientPersonalDetailId
                                                select new ClientDetailResponseDto
                                                {
                                                    ClientId = clientPersonalDetail.ClientId,
                                                    FirstName = clientPersonalDetail.FirstName,
                                                    MiddleName = clientPersonalDetail.MiddleName,
                                                    LastName = clientPersonalDetail.LastName,
                                                    Address = clientPersonalDetail.Address,
                                                    PhoneNumber = clientPersonalDetail.PhoneNumber,
                                                    Email = clientPersonalDetail.Email,
                                                    ProductId = clientPersonalDetail.ProductId,
                                                    ProductName = product.Name,
                                                }).FirstOrDefault();

                Logger.LogInformation($"GetClientDetailById responded for User: {CurrentUser.Id}");

                return clientPersonalDetailData;
            }
            catch (Exception)
            {
                Logger.LogError(nameof(GetClientDetailById));
                throw;
            }
        }

        public async Task<ResponseDto<ClientDetailResponseDto>> UpdateClientDetail(Guid clientPersonalDetailId, UpdateClientDetailDto input)
        {
            try
            {
                Logger.LogInformation($"UpdateClientDetail requested by User: {CurrentUser.Id}");
                Logger.LogDebug($"UpdateClientDetail requested for User: {(CurrentUser.Id, input)}");

                var clientPersonalDetailQuery = await _clientPersonalDetailRepository.GetQueryableAsync();
                var productQuery = await _productRepository.GetQueryableAsync();

                var clientPersonalDetail = clientPersonalDetailQuery.Where(x => x.Id == clientPersonalDetailId).FirstOrDefault()
                    ?? throw new UserFriendlyException("Client Personal Detail not found", code: "400");


                clientPersonalDetail.FirstName = input.FirstName;
                clientPersonalDetail.MiddleName = input.MiddleName;
                clientPersonalDetail.LastName = input.LastName;
                clientPersonalDetail.Address = input.Address;
                clientPersonalDetail.PhoneNumber = input.PhoneNumber;

                await _clientPersonalDetailRepository.UpdateAsync(clientPersonalDetail);

                var clientPersonalDetailData = (from clientPersonalDetails in clientPersonalDetailQuery
                                                join product in productQuery on clientPersonalDetails.ProductId equals product.Id into productLeft
                                                from product in productLeft.DefaultIfEmpty()
                                                where clientPersonalDetail.Id == clientPersonalDetailId
                                                select new ClientDetailResponseDto
                                                {
                                                    ClientId = clientPersonalDetail.ClientId,
                                                    FirstName = clientPersonalDetail.FirstName,
                                                    MiddleName = clientPersonalDetail.MiddleName,
                                                    LastName = clientPersonalDetail.LastName,
                                                    Address = clientPersonalDetail.Address,
                                                    PhoneNumber = clientPersonalDetail.PhoneNumber,
                                                    Email = clientPersonalDetail.Email,
                                                    ProductId = clientPersonalDetail.ProductId,
                                                    ProductName = product.Name,
                                                }).FirstOrDefault();

                var response = new ResponseDto<ClientDetailResponseDto>
                {
                    Success = true,
                    Code = 200,
                    Message = "New Client Details updated successfully",
                    Data = clientPersonalDetailData
                };

                Logger.LogInformation($"UpdateClientDetail responded for User: {CurrentUser.Id}");

                return response;
            }
            catch (Exception)
            {
                Logger.LogError(nameof(UpdateClientDetail));
                throw;
            }
        }

        public async Task<PagedResultDto<ClientDetailResponseDto>> GetPagedAndSortedClientDetailAsync(PagedAndSortedClientDetailListDto input)
        {
            try
            {
                var clientPersonalDetailQuery = await _clientPersonalDetailRepository.GetQueryableAsync();
                var productQuery = await _productRepository.GetQueryableAsync();


                if (input.Sorting.IsNullOrWhiteSpace())
                {
                    input.Sorting = "CreationTime";
                }

                input.Sorting = $"{input.Sorting} {input.SortOrder}";

                if (!string.IsNullOrWhiteSpace(input.SearchKeyword))
                {
                    clientPersonalDetailQuery = clientPersonalDetailQuery.Where(x =>
                        x.ClientId.ToString().Contains(input.SearchKeyword.ToLower()) ||
                        x.FirstName.ToLower().Contains(input.SearchKeyword.ToLower()) ||
                        x.MiddleName.ToLower().Contains(input.SearchKeyword.ToLower()) ||
                        x.LastName.ToLower().Contains(input.SearchKeyword.ToLower()) ||
                        x.Address.ToLower().Contains(input.SearchKeyword.ToLower()) ||
                        x.Email.ToLower().Contains(input.SearchKeyword.ToLower()));
                }

                if (input.ProductId != null)
                {
                    clientPersonalDetailQuery = clientPersonalDetailQuery.Where(x => x.ProductId == input.ProductId);
                }


                var query = (from clientDetail in clientPersonalDetailQuery
                             join product in productQuery on clientDetail.ProductId equals product.Id into productLeft
                             from product in productLeft.DefaultIfEmpty()
                             select new ClientDetailResponseDto
                             {
                                 Id = clientDetail.Id,
                                 FirstName = clientDetail.FirstName,
                                 MiddleName = clientDetail.MiddleName,
                                 LastName = clientDetail.LastName,
                                 Address = clientDetail.Address,
                                 PhoneNumber = clientDetail.PhoneNumber,
                                 Email = clientDetail.Email,
                                 ProductId = clientDetail.ProductId,
                                 ProductName = product.Name,
                             });

                var result = query.OrderBy(input.Sorting)
                                  .Skip(input.SkipCount)
                                  .Take(input.MaxResultCount).ToList();

                var totalCount = query.Count();
                var response = new PagedResultDto<ClientDetailResponseDto>(totalCount, result);

                return response;
            }
            catch (Exception)
            {
                Logger.LogError(nameof(GetPagedAndSortedClientDetailAsync));
                throw;
            }
        }
    }
}
