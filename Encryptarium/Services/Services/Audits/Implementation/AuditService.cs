using BusinessLogic.Entities;
using BusinessLogic.Services.Audits.Interface;
using BusinessLogic.Services.Base;
using DAL.Repositories.Audits.Interface;
using Microsoft.Extensions.Logging;
using Model;
using Model.Entities;
using Model.Enums;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BusinessLogic.Services.Audits.Implementation
{
    public class AuditService : BaseService, IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        public AuditService(IAuditRepository auditRepository, ILogger<AuditService> logger) : base(logger) 
        {
            _auditRepository = auditRepository;
        }

        public async Task CreateAsync(Audit audit)
        {
            await _auditRepository.CreateAsync(audit);
        }

        public async Task<ServiceResponse<ResponseAuditDTOs>> GetAll(FilterAttributeAudit filterAttributeAudit)
        {
            Expression<Func<Audit, bool>> lambda;

            var argument = Expression.Parameter(typeof(Audit), "audit");

            var conditions = new List<Expression>();

            if (filterAttributeAudit.UserUid is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.UserUid));
                var value = Expression.Constant(filterAttributeAudit.UserUid, typeof(Guid?));
                var condition = Expression.Condition(
                    Expression.Equal(value, Expression.Constant(null, typeof(Guid?))),
                    Expression.Equal(property, Expression.Constant(null, typeof(Guid?))),
                    Expression.Equal(property, value)
                );
                //var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.Microservice is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.SourceMicroservice));
                var value = Expression.Constant(filterAttributeAudit.Microservice);
                var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.Controller is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.SourceController));
                var value = Expression.Constant(filterAttributeAudit.Controller);
                var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.Method is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.SourceMethod));
                var value = Expression.Constant(filterAttributeAudit.Method);
                var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.AuthorizePolicies is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.AuthorizePolice));
                var value = Expression.Constant(filterAttributeAudit.AuthorizePolicies);
                var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.StatusCode is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.StatusCode));
                var value = Expression.Constant(filterAttributeAudit.StatusCode);
                var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.Entity is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.Entity));
                var value = Expression.Constant(filterAttributeAudit.Entity);
                var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }

            if (filterAttributeAudit.EntityUid is not null)
            {
                var property = Expression.Property(argument, nameof(Audit.EntityUid));
                var value = Expression.Constant(filterAttributeAudit.EntityUid, typeof(Guid?));
                var condition = Expression.Condition(
                    Expression.Equal(value, Expression.Constant(null, typeof(Guid?))),
                    Expression.Equal(property, Expression.Constant(null, typeof(Guid?))),
                    Expression.Equal(property, value)
                );
                //var condition = Expression.Equal(property, value);
                conditions.Add(condition);
            }
            // Лямбда для времени
            var propertyDateAct = Expression.Property(argument, nameof(Audit.DateAct));
            var valueDateStart = Expression.Constant(filterAttributeAudit.DateStart);
            var conditionDateStart = Expression.GreaterThanOrEqual(propertyDateAct, valueDateStart);
            conditions.Add(conditionDateStart);
            var valueDateEnd = Expression.Constant(filterAttributeAudit.DateEnd);
            var conditionDateEnd = Expression.LessThanOrEqual(propertyDateAct, valueDateEnd);
            conditions.Add(conditionDateEnd);

            var body = conditions.Aggregate(Expression.AndAlso);
            lambda = Expression.Lambda<Func<Audit, bool>>(body, argument);
            var audits = await _auditRepository.GetAllAsync(lambda);
            audits = audits.OrderByDescending(a => a.DateAct).ToList();
            int countPage = audits.Count() / Constants.Take;
            countPage += audits.Count() % Constants.Take == 0 ? 0 : 1;
            var responseAuditDTOs = new ResponseAuditDTOs()
            {
                Audits = audits.Skip(filterAttributeAudit.Skip).Take(filterAttributeAudit.Take).OrderByDescending(a => a.DateAct).ToList(),
                Count = countPage,
                CurrentPage = filterAttributeAudit.Skip / Constants.Take + 1
            };
            return Ok(responseAuditDTOs);
        }

        public async Task<Audit> GetAsync(Guid uid)
        {
            return await _auditRepository.GetAsync(uid);
        }
    }
}
