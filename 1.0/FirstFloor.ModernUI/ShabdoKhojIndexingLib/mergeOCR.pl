#!/usr/bin/perl
use strict;
use warnings;
use Data::Dumper;
use JSON;

# (1) quit unless we have the correct number of command-line args
my $num_args = $#ARGV + 1;
if ($num_args != 3) {
    print "\nUsage: perl mergeOCR.pl <OCR Input Dir> <ignor File List Name> <JSON File Name> \n";
    exit;
}
my $ocrDir = $ARGV[0];
my $ignorFileName = $ARGV[1];
my $jsonOutFile = $ARGV[2];
opendir DIR, $ocrDir;
my @files = grep { $_ ne '.' && $_ ne '..' } readdir DIR;
closedir DIR;

#print Dumper \@files;
#my @files = @ARGV;
my @tagArray;

my $ignorFile = $ignorFileName;
open (FH, "< $ignorFile") or die "Can't open $ignorFile for read: $!";
my @ignor;
while (<FH>) {
    chomp;
    push (@ignor, $_);
}
close FH;

foreach my $filename (@files){
    if ( -s "$ocrDir/$filename" ) {
        open FH, "$ocrDir/$filename" or die;
        $filename =~ m/(foo-)(.+?)(.txt)/;
        my $startTime = $2;
        my @tag;
        my $durTime = "1";
        while (<FH>) {
            chomp;
            my @mainTag = split(' ',$_);
            print Dumper \@mainTag;
            foreach my $varTemp (@mainTag){
                #$varTemp =~ s/[\$#@~!&*()\[\][-];.,:?^ `\\\/]+//g;
                $varTemp =~ s/\W//g;
                push (@tag, $varTemp) if ($varTemp);
            }
            
        }
        my %temp;
        @temp{@ignor} = undef;
        @tag = grep {not exists $temp{$_}} @tag;
        my $tags = join(' ', @tag);
        push @tagArray,{ start => $startTime, dur => $durTime, tags => $tags };
        close FH;
    }    
}

my $jsonTags = new JSON;
open my $FH, ">", $jsonOutFile;
print $FH $jsonTags->pretty->encode(\@tagArray);
close $FH;