using System;
using System.Collections.Generic;
using System.Text;

namespace pidgeon_sv
{
    class Item
    {
        public DateTime _date;
        string _text;
        public Item(string Data)
        {
            _date = DateTime.Now;
            _text = Data;
        }
    }

    class OutgoingQueue
    {

        List<Item> queue = new List<Item>();
    }

    class IncomingQueue
    {
        List<Item> queue = new List<Item>();
    }

    class Connection
    {
        public string name;
        public string host;
        public string user;
        public Account account = null;
        public Status status = Status.WaitingPW;
        public System.Threading.Thread queue;
        public System.IO.StreamReader _r;
        public System.IO.StreamWriter _w;
        public OutgoingQueue _outgoing = new OutgoingQueue();
        public IncomingQueue _incoming = new IncomingQueue();

        public bool Active = true;

        public enum Status {
            WaitingPW,
            Connected,
            Disconnected,
        }

        public bool Mode = false;


    }
}
