using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MISA.AMIS.WebApi.DL;
using Npgsql;

namespace MISA.AMIS.WebApi.DL
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private DbConnection? _connection = null;
        private DbTransaction? _transaction = null;

        private readonly string _connectionString;

        public UnitOfWork(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"] ?? "";
        }

        public DbConnection Connection => _connection ??= new NpgsqlConnection(_connectionString);

        public DbTransaction? Transaction => _transaction;

        public DbTransaction DbTransaction => throw new NotImplementedException();

        public async Task<DbConnection> OpenConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
            return _connection;
        }

        public async Task BeginTransactionAsync()
        {
            _connection ??= new NpgsqlConnection(_connectionString);

            if (_connection.State == ConnectionState.Open)
            {
                _transaction = await _connection.BeginTransactionAsync();
            }
            else
            {
                await _connection.OpenAsync();
                _transaction = await _connection.BeginTransactionAsync();
            }

        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
            else DisposeAsync();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
            else await DisposeAsync();
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
