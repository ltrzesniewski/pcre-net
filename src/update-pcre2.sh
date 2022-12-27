#!/usr/bin/env bash

# To update PCRE2:
# - Delete the contents of the PCRE directory
# - Extract the PCRE2 release into the PCRE directory
# - Run this script

set -e

cd "$(dirname "$0")"
cd PCRE/src

cp pcre2.h.generic pcre2.h
cp pcre2_chartables.c.dist pcre2_chartables.c

{
  echo
  echo '#include "../../PCRE.NET.Native/pcre2config.h"'
  echo
  cat config.h.generic
} > config.h

echo 'PCRE2 release patched successfully.'
