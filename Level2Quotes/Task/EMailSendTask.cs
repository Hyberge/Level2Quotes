using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.Task
{
    class EMailSendTask: ITask
    {
        String mSubject;
        String mMessage;

        public EMailSendTask(ITask Next): base(Next)
        { }

        public override bool TransactionProcessing()
        {
            return EMailSender.SenderMessage(mSubject, mMessage);
        }

        public void SetEmailContents(String Subject, String Message)
        {
            mSubject = Subject;
            mMessage = Message;
        }
    }
}
