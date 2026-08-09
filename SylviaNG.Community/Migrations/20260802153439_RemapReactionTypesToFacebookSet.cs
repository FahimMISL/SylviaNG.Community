using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Community.Migrations
{
    /// <inheritdoc />
    public partial class RemapReactionTypesToFacebookSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Old reaction set (Like, Love, Celebrate, Support, Insightful) is being replaced by
            // Facebook's set (Like, Love, Care, Haha, Wow, Sad, Angry). Remap existing rows so no
            // reaction becomes an invalid/orphaned value once ReactionTypeEnum drops the old members.
            migrationBuilder.Sql(
                """
                UPDATE "PostReactions" SET "ReactionType" = 'Haha' WHERE "ReactionType" = 'Celebrate';
                UPDATE "PostReactions" SET "ReactionType" = 'Like' WHERE "ReactionType" = 'Support';
                UPDATE "PostReactions" SET "ReactionType" = 'Wow' WHERE "ReactionType" = 'Insightful';

                UPDATE "CommentReactions" SET "ReactionType" = 'Haha' WHERE "ReactionType" = 'Celebrate';
                UPDATE "CommentReactions" SET "ReactionType" = 'Like' WHERE "ReactionType" = 'Support';
                UPDATE "CommentReactions" SET "ReactionType" = 'Wow' WHERE "ReactionType" = 'Insightful';

                UPDATE "RecognitionReactions" SET "ReactionType" = 'Haha' WHERE "ReactionType" = 'Celebrate';
                UPDATE "RecognitionReactions" SET "ReactionType" = 'Like' WHERE "ReactionType" = 'Support';
                UPDATE "RecognitionReactions" SET "ReactionType" = 'Wow' WHERE "ReactionType" = 'Insightful';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Best-effort reverse remap. Lossy: if multiple old values mapped to the same new value
            // (none do here) or a row was later changed to Haha/Wow/etc. by a user, this can't tell
            // that apart from a remapped row - acceptable for a rollback of this data migration.
            migrationBuilder.Sql(
                """
                UPDATE "PostReactions" SET "ReactionType" = 'Celebrate' WHERE "ReactionType" = 'Haha';
                UPDATE "PostReactions" SET "ReactionType" = 'Support' WHERE "ReactionType" = 'Like';
                UPDATE "PostReactions" SET "ReactionType" = 'Insightful' WHERE "ReactionType" = 'Wow';

                UPDATE "CommentReactions" SET "ReactionType" = 'Celebrate' WHERE "ReactionType" = 'Haha';
                UPDATE "CommentReactions" SET "ReactionType" = 'Support' WHERE "ReactionType" = 'Like';
                UPDATE "CommentReactions" SET "ReactionType" = 'Insightful' WHERE "ReactionType" = 'Wow';

                UPDATE "RecognitionReactions" SET "ReactionType" = 'Celebrate' WHERE "ReactionType" = 'Haha';
                UPDATE "RecognitionReactions" SET "ReactionType" = 'Support' WHERE "ReactionType" = 'Like';
                UPDATE "RecognitionReactions" SET "ReactionType" = 'Insightful' WHERE "ReactionType" = 'Wow';
                """);
        }
    }
}
