﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FazCtrl.EventSourcing.Data;

namespace FazCtrl.EventSourcing.Interfaces
{

    public interface IEventStore
    {
        Task<IEnumerable<EventData>> Load(string partitionKey, int version);

        void Save(string partitionKey, IEnumerable<EventData> events);
    }



    public interface IEventSourcedRepository<T> where T : IEventSourced
    {
        /// <summary>
        /// Tries to retrieve the event sourced entity.
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <returns>The hydrated entity, or null if it does not exist.</returns>
        Task<T> Find(Guid id);

        /// <summary>
        /// Retrieves the event sourced entity.
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <returns>The hydrated entity</returns>
        /// <exception cref="EntityNotFoundException">If the entity is not found.</exception>
        T Get(Guid id);

        /// <summary>
        /// Saves the event sourced entity.
        /// </summary>
        /// <param name="eventSourced">The entity.</param>
        /// <param name="correlationId">A correlation id to use when publishing events.</param>
        void Save(T eventSourced, string correlationId);
    }
}

