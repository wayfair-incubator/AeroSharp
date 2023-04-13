# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2023-04-11

### Changed

Introduced fetching of an Array of Node for an aerospike client.

## [1.0.0] - 2023-03-29

### Changed

- Target .net6 and .net7 frameworks
- `Aerospike.Client` v5.1.1 => 6.0.0

Our first major release!  This looks to upgrade the package with some of the performance improvements in .net6 and
includes all of the additional changes/improvements from Aerospike in their latest client version.

## [0.4.0] - 2023-03-29

### Changed

- Allow users to set MaxConcurrentThreads in ReadConfiguration

## [0.3.1] - 2022-06-01

### Changed

- `protobuf-net` v3.1.0 => v3.1.4
- `Aerospike.Client` v5.0.0 => v5.1.1
- `FluentValidation` v11.0.1 => v11.0.2

## [0.3.0] - 2022-04-13

### Changed

- `FluentValidation` v10.4.0 => v11.0.1
- `protobuf-net` v3.0.101 => v3.1.0

## [0.2.0] - 2022-04-13

### Changed

- `Aerospike.Client` v4.2.7 => v5.0.0.

See Aerospike's [C# Client Library Release Notes](https://download.aerospike.com/download/client/csharp/notes.html#5.0.0) for details on underlying core client changes.

## [0.1.2] - 2022-03-28

### Added

- Added README to package contents

### Changed

- `FluentValidation` v10.3.6 => v10.4.0

## [0.1.1] - 2022-01-31

### Changed

- `FluentValidation` v10.3.5 => v10.3.6
- `Aerospike.Client` v4.2.6 => v4.2.7
- `Polly` v7.2.2 => v7.2.3

## [0.1.0] - 2021-12-10

Initial release.

See the [README](https://github.com/wayfair-incubator/AeroSharp/blob/main/README.md) for more information.
