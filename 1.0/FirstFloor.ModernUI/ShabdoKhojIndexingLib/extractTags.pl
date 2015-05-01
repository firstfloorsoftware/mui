#!/usr/bin/perl
use strict;
use XML::Simple;
use Data::Dumper;
use JSON;
my $key;
my $simple = XML::Simple->new();
my $data = $simple->XMLin('sample.xml', 'ContentKey' => '-root-contents');
my @tagArray;
my $ignorFile = "ignor.txt";
open (FH, "< $ignorFile") or die "Can't open $ignorFile for read: $!";
my @ignor;
while (<FH>) {
    chomp;
    push (@ignor, $_);
}
close FH or die "Cannot close $ignorFile: $!";
foreach my $part (@{$data->{text}}) {
	my $startTime = $part->{'start'};
	my $durTime = $part->{'dur'};
	my @mainTag = split(' ',$part->{'root-contents'});
	my %temp;
        @temp{@ignor} = undef;
        @mainTag = grep {not exists $temp{$_}} @mainTag;
        my $tags = join(' ', @mainTag);
	push @tagArray,{ start => $startTime, dur => $durTime, tags => $tags }; 
}
my $jsonTags = new JSON;
open my $fh, ">", "sample1.json";
print $fh $jsonTags->pretty->encode(\@tagArray);
close $fh;