using System;
using System.ComponentModel;

namespace Hanoi.Authentication.Messenger
{
    [Flags]
    public enum Scope
    {
        [Description("wl.basic")]
        Basic = 1,

        [Description("wl.messenger")]
        Messenger = 2,

        [Description("wl.offline_access")]
        Offline = 4,

        [Description("wl.signin")]
        Signin = 8,

        [Description("wl.birthday")]
        Birthday = 16,

        [Description("wl.calendars")]
        Calendars = 32,

        [Description("wl.calendars_update")]
        CalendarsUpdate = 64,

        [Description("wl.contacts_birthday")]
        ContactsBirthday = 128,

        [Description("wl.contacts_create")]
        ContactsCreate = 256,

        [Description("wl.contacts_calendars")]
        ContactsCalendars = 512,

        [Description("wl.contacs_photos")]
        ContactsPhotos = 1024,

        [Description("wl.contacts_skydrive")]
        ContactsSkydrive = 2048,

        [Description("wl.emails")]
        Emails = 4096,

        [Description("wl.events_create")]
        EventsCreate = 8192,

        [Description("wl.phone_numbers")]
        PhoneNumbers = 16384,

        [Description("wl.photos")]
        Photos = 32768,

        [Description("wl.postal_addresses")]
        PostalAddresses = 65536,

        [Description("wl.share")]
        Share = 131072,

        [Description("wl.skydrive")]
        Skydrive = 262144,

        [Description("wl.skydrive_update")]
        SkydriveUpdate = 524288,

        [Description("wl.work_profile")]
        WorkProfile = 1048576,

        [Description("wl.applications")]
        Applications = 2097152,

        [Description("wl.applications_create")]
        ApplicationsCreate = 4194304
    }
}