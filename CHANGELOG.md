# Changelog

All notable changes to the SylviaNG.Community backend are documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). Earlier history
is available in the git log and merged pull requests #1 to #9.

## [Unreleased]

### Added
- Elections: endpoints to remove a candidate and to update a candidate's manifesto.
- Elections: `PublishedAt` column (migration `AddElectionPublishedAt`).
- Teams: query to fetch teams by employee id (`TeamGetByEmployeeId`).
- Migrations `AddEmployeePhotoFileIdsAndGroupAvatarFk` (employee photo file ids, group avatar foreign key)
  and `AddFilePathToChatMessageAttachments`.
- Project documents: `LICENSE`, `SECURITY.md`, `CONTRIBUTING.md`, `CHANGELOG.md`,
  `TERMS_PRIVACY_POLICY.md` (draft), pull request template, `appsettings.Example.json`, `.dockerignore`.

### Changed
- `database_script.sql` regenerated to include all current migrations.
- Service, mapping, entity configuration and controller updates across Teams, Tasks, Elections and report-resolve flows.

### Removed
- Kafka employee-event integration (`EmployeeEventConsumer`, `KafkaSettings`, the `Confluent.Kafka`
  package and the `Kafka` configuration section).
