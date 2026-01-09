using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
    interface IPage
    {
        abstract public void OnChangeLanguage(object sender, string selectedLanguague);
	}
}
