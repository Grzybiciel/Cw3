﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public int idEnrollment { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }

        public int IdStudy { get; set; }
    }
}