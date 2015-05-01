use strict;
use warnings;
use Data::Dumper;

# (1) quit unless we have the correct number of command-line args
my $num_args = $#ARGV + 1;
if ($num_args != 5) {
    print "\nUsage: perl generateFilePlayer.pl <Path> <video file> <video file name> <video type> <Json file>\n";
    exit;
}
my $shabdoPath = $ARGV[0]; 
my $filename = $shabdoPath.'demo.html';
my $videoInputFile = $ARGV[1];
my $videoFileName = $ARGV[2];
my $videoType = $ARGV[3];
my $jsonFileName = $ARGV[4];

my $outFileName = $shabdoPath.$videoFileName.'.html';


#print Dumper $shabdoPath;
#print Dumper $filename;
#print Dumper $videoInputFile;
#print Dumper $videoType;
#print Dumper $jsonFileName;
#print Dumper $outFileName;
my $data = read_file($filename);
$data =~ s/shobdokhojVideoFileName/$videoInputFile/g;
$data =~ s/shobdokhojVideoFileType/$videoType/g;
$data =~ s/shobdokhojJsonFileName/$jsonFileName/g;
write_file($outFileName, $data);
exit;
 
sub read_file {
    my ($filename) = @_;
 
    open my $in, '<:encoding(UTF-8)', $filename or die "Could not open '$filename' for reading $!";
    local $/ = undef;
    my $all = <$in>;
    close $in;
 
    return $all;
}
 
sub write_file {
    my ($filename, $content) = @_;
 
    open my $out, '>:encoding(UTF-8)', $filename or die "Could not open '$filename' for writing $!";;
    print $out $content;
    close $out;
 
    return;
}
