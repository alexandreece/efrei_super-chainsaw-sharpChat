using System;

namespace super_chainsaw_sharpChatClient
{
    internal class MessagesWriter : RtfWriter
    {
        public enum ColorNames
        {// warning: these names must always match the order in which the colors are added to the list
         // (RTF format is such that 0 is always black and the first color in the list is indexed by 1)

            unused_color,// black
            notification,// lightblue
            technicalDetails,// grey
            messageHeader,// darkblue
            messageContent,// green
            issueOrBadEnd// red
        }

        private string lastUsername;
        private DateTime lastDate;

        public MessagesWriter() : base(new Colors().add(new Color(20, 120, 150))// warning: upon modifying this list of colors,
                                                   .add(new Color(80, 80, 80))  // also modify the enumeration of names so that
                                                   .add(new Color(20, 20, 100)) // the names still match the colors in the list
                                                   .add(new Color(20, 100, 20))
                                                   .add(new Color(200, 20, 20)))
        { }

        public MessagesWriter notify(string notification, string technicalDetails)
        {
            newline().newline().color((int) ColorNames.notification);
            text(notification);
                
            newline().color((int) ColorNames.technicalDetails);
            text(technicalDetails);

            return this;
        }

        public MessagesWriter usernameAtDate(string username, DateTime date)
        {
            bool sameMinute = (date.Year == lastDate.Year
                            && date.Month == lastDate.Month
                            && date.Day == lastDate.Day
                            && date.Hour == lastDate.Hour
                            && date.Minute == lastDate.Minute);
            bool sameUsername = (username == lastUsername);// todo : check no problem because of "==" instead of ".Equals()"

            newline();

            if (!sameMinute || !sameUsername)
            {
                newline().color((int) ColorNames.messageHeader).text(username);

                if (!sameMinute)
                    color((int) ColorNames.technicalDetails).text(" @ " + date.Hour + ":" + date.Minute);
            }

            lastUsername = username;
            lastDate = date;

            return this;
        }

        public MessagesWriter message(string message)
        {
            newline().color((int) ColorNames.messageContent).text(message);

            return this;
        }
    }
}
