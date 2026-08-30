namespace SylviaNG.Community.Domain.Enums;

public enum AnnouncementStatusEnum
{
    Draft,
    Published,
    Scheduled,
    Expired,
    Archived
}

public enum AnnouncementTypeEnum
{
    General,
    Holiday,
    Event,
    Policy,
    Emergency,
    Achievement
}

public enum ContactVisibilityEnum
{
    Private,
    Public
}

public enum VisibilityEnum
{
    Everyone,
    Department,
    Branch
}

public enum ReactionTypeEnum
{
    Like,
    Love,
    Care,
    Haha,
    Wow,
    Sad,
    Angry
}

public enum GroupVisibilityEnum
{
    Public,
    Private
}

public enum GroupMemberRoleEnum
{
    Member,
    Contributor,
    GroupAdmin,
    Creator
}

public enum GroupJoinRequestStatusEnum
{
    Pending,
    Approved,
    Rejected
}

public enum TodayEventTypeEnum
{
    Birthday,
    Anniversary
}

// Module 11 - Messenger
public enum ConversationTypeEnum
{
    Direct,
    Group
}

public enum MessageTypeEnum
{
    Text,
    Attachment,
    Voice,
    Shared,
    System
}

public enum SharedContentTypeEnum
{
    Post,
    Listing,
    Event
}

public enum ChatAttachmentTypeEnum
{
    Image,
    File,
    Voice
}