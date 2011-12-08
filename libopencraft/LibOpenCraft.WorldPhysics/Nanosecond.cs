using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.WorldPhysics
{
    public class Nanosecond
    {
        private int microsecond;
        private int m_count;
        private DateTime time;
        public Nanosecond()
        {
            microsecond = 0;
            m_count = 0;
            time = new DateTime();
        }
        public bool RunMiliSecond()
        {
            if (DateTime.Now.Millisecond > time.Millisecond || DateTime.Now.Millisecond < time.Millisecond)
            {
                time = DateTime.Now;
                m_count++;
            }

            if (m_count == 4)
            {
                m_count = 0;
                return true;
            }
            else
                return false;
        }
    }
}
