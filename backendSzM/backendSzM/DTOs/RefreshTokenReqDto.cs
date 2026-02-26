using System;
using System.Collections.Generic;
using System.Text;

namespace backendSzM.DTOs
{
    public  class RefreshTokenReqDto
    {
        public Guid Id { get; set; }
        
        public required string RefreshToken { get; set; }
    }
}
