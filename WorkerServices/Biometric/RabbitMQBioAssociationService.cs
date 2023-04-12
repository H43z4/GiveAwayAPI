using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.Biometric;
using Models.ViewModels.VehicleRegistration.Core;
using Newtonsoft.Json;
using Profiling;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RepositoryLayer;
using SharedLib.Common;
using SharedLib.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Biometric
{
    public class RabbitMQBioAssociationService : BackgroundService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;
        readonly IProfilingService profilingService;

        readonly RabbitMQServerConfig rabbitMQServerConfig;

        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private string queue;

        public RabbitMQBioAssociationService(IOptions<RabbitMQServerConfig> rabbitMQServerConfig, IProfilingService profilingService, AppDbContext appDbContext, IAdoNet adoNet)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
            this.profilingService = profilingService;

            this.rabbitMQServerConfig = rabbitMQServerConfig.Value;
            this.queue = this.rabbitMQServerConfig.Channels.FirstOrDefault(x => x.Name == "CH-BIOMETRIC").Queues.SingleOrDefault(x => x.Key == "Q-BIO-ASSOCIATION").Value;
            //this.queue = this.rabbitMQServerConfig.Channels.FirstOrDefault(x => x.Name == "CH-BIOMETRIC").Queues.SingleOrDefault(x => x.Key == "Q-BIO-BUSINESS").Value;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this._connectionFactory = new ConnectionFactory
            {
                HostName = this.rabbitMQServerConfig.HostName,
                UserName = this.rabbitMQServerConfig.UserName,
                Password = this.rabbitMQServerConfig.Password,
                Port = this.rabbitMQServerConfig.Port,

                DispatchConsumersAsync = true
            };

            this._connection = this._connectionFactory.CreateConnection();
            this._channel = this._connection.CreateModel();

            this._channel.QueueDeclare(queue: this.queue,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

            //channel.QueueDeclare(queue);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(this._channel);

            consumer.Received += async (bc, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var repAssociation = JsonConvert.DeserializeObject<VwRepAssociation>(message);

                    await this.SaveBusinessRep(repAssociation);

                    await this.SaveAssociation(repAssociation);

                    //this._channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException)
                {
                    this._channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            this._channel.BasicConsume(queue: this.queue, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            this._connection.Close();
        }


        #region AMQP-Organizational-Rep-Association

        private async Task CommitTransaction()
        {
            using (var transaction = this.appDbContext.Database.BeginTransaction())
            {
                var rowsAffected = await this.appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        public async Task<long> SaveAssociation(VwRepAssociation vwRepAssociation)
        {
            SqlParameter[] sql = new SqlParameter[4];

            sql[0] = new SqlParameter("@ApplicationId", vwRepAssociation.ApplicationId);
            sql[1] = new SqlParameter("@VehicleId", vwRepAssociation.VehicleId);
            sql[2] = new SqlParameter("@OwnerGroupId", vwRepAssociation.OwnerGroupId);
            sql[3] = new SqlParameter("@PersonId", vwRepAssociation.PersonId);

            var rowAffected = await this.adoNet.ExecuteNonQuery("[Biometric].[SaveAssociation]", sql);

            return rowAffected;
        }

        public async Task<BusinessRep> SaveBusinessRep(VwRepAssociation vwRepAssociation)
        {
            var ds = await this.profilingService.GetPersonByCNIC(vwRepAssociation.Person.CNIC);

            var person = ds.Tables[0].ToList<VwPerson>().FirstOrDefault();

            if (person is null)
            {
                var personObj = await this.profilingService.SavePerson(vwRepAssociation.Person);

                person = new VwPerson();
                vwRepAssociation.PersonId = personObj.PersonId;
            }

            var businessRep = this.appDbContext.BusinessRep.SingleOrDefault(x => x.BusinessId == vwRepAssociation.BusinessId && x.PersonId == vwRepAssociation.PersonId);

            if (businessRep is null)
            {
                businessRep = new BusinessRep()
                {
                    PersonId = vwRepAssociation.PersonId.Value,
                    BusinessId = vwRepAssociation.BusinessId,
                    BusinessRepStatusId = 1,
                    CreatedBy = 1,
                    Designation = vwRepAssociation.Designation,
                    Title = vwRepAssociation.Title
                };

                this.appDbContext.BusinessRep.Add(businessRep);

                await this.CommitTransaction();
            }

            return businessRep;
        }

        #endregion
    }
}
