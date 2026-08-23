namespace SylviaNG.Community.Application.Features.Surveys
{
    /// <summary>
    /// Whitelisted values for SurveyQuestion.QuestionType, mirroring SurveyAudienceTypes'
    /// plain-string-plus-whitelist convention (no C# enum in the Surveys feature). This class
    /// is the single source of truth for the values SurveyQuestionAddValidator/
    /// SurveyQuestionUpdateValidator accept - values must match the frontend's
    /// SurveyQuestionType union (survey.interface.ts) and QUESTION_TYPE_OPTIONS
    /// (survey-builder.component.ts) exactly.
    /// </summary>
    public static class SurveyQuestionTypes
    {
        public const string SingleChoice = "SingleChoice";
        public const string MultipleChoice = "MultipleChoice";
        public const string Text = "Text";
        public const string Rating = "Rating";

        public static readonly string[] All = { SingleChoice, MultipleChoice, Text, Rating };

        /// <summary>Question types that are answered via SurveyOption selection, not free text.</summary>
        public static readonly string[] ChoiceTypes = { SingleChoice, MultipleChoice };
    }
}
