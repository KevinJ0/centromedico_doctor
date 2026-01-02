using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CentromedicoDoctor.Services.Interfaces;

namespace CentromedicoDoctor.Services
{
    public class S3Servicea : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Servicea(IAmazonS3 client)
        {
            _client = client;
        }
    }
}
