﻿using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithRest.Tests.Infrastructure
{
    internal class FakeTrustListRepository : ITrustListRepository
    {
        //public ApplicationName AddApplicationName(ApplicationName applicationName)
        //{
        //    //ToDo: Implement
        //    //throw new NotImplementedException();
        //    return applicationName;
        //}

        //public Task<IEnumerable<ApplicationName>> GetAllApplicationNames()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<ApplicationName>> GetApplicationNamesByApplicationId(int id)
        //{
        //    //ToDo implement
        //    return Task.Run(() =>
        //    {
        //        return (IEnumerable<ApplicationName>)new List<ApplicationName>();
        //    });
        //}

        //public void RemoveApplicationNames(ApplicationName[] applicationNames)
        //{
        //    //ToDo implement
        //    return;
        //}
        public TrustList AddTrustList(TrustList trustList)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TrustList>> GetTrustListsByApplicationId(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveTrustLists(TrustList[] trustLists)
        {
            return;
        }

        public void SaveChanges(TrustList trustList)
        {
            throw new NotImplementedException();
        }
    }
}
